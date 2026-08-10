// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;
using cCoder.Eventing.Models;
using Moq;
using System.Text.Json;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations.Events;

public sealed partial class EventHandlerServiceTests
{
    [Fact]
    public async Task ShouldHandleSerializedContentPagesImportedEventAsync()
    {
        // Given
        Mock<IEventHubBroker> eventHubBrokerMock = new(behavior: MockBehavior.Loose);
        Mock<IAppSecurityMigrationAggregationService> migrationServiceMock = new();
        Func<IAppSecurityMigrationAggregationService, AppSecurityPackageEvent, ValueTask> actualHandler = null;
        const int expectedAppId = 731;

        Package expectedPackage = new()
        {
            Id = Guid.NewGuid(),
            Name = "Page roles"
        };

        EventMessage<AppSecurityPackageEvent> outboundMessage = new()
        {
            Data = new AppSecurityPackageEvent
            {
                AppId = expectedAppId,
                Package = expectedPackage
            }
        };

        eventHubBrokerMock.Setup(expression: broker => broker.ListenToEvent<
                AppSecurityPackageEvent,
                IAppSecurityMigrationAggregationService>(
                    eventName: "content_pages_imported",
                    handler: It.IsAny<Func<
                        IAppSecurityMigrationAggregationService,
                        AppSecurityPackageEvent,
                        ValueTask>>()))
            .Callback<string, Func<
                IAppSecurityMigrationAggregationService,
                AppSecurityPackageEvent,
                ValueTask>>(action: (_, handler) => actualHandler = handler);

        EventHandlerService eventHandlerService = new(
            eventHubBroker: eventHubBrokerMock.Object);

        // When
        eventHandlerService.ListenToPackageEvents();

        string httpData = JsonSerializer.Serialize(value: outboundMessage.Data);

        AppSecurityPackageEvent inboundEvent =
            JsonSerializer.Deserialize<AppSecurityPackageEvent>(json: httpData);

        await actualHandler(
            arg1: migrationServiceMock.Object,
            arg2: inboundEvent);

        // Then
        migrationServiceMock.Verify(
            expression: service => service.ImportPageRolesAppSecurityPackageAsync(
                appId: expectedAppId,
                package: It.Is<AppSecurityPackage>(match: package =>
                    package.Id == expectedPackage.Id &&
                    package.Name == expectedPackage.Name)),
            times: Times.Once);
    }
}