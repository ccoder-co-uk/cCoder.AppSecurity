// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations.Events;

public sealed partial class EventHandlerServiceTests
{
    [Fact]
    public void ShouldListenForContentPagesImportedEvent()
    {
        // Given
        Mock<IEventHubBroker> eventHubBrokerMock = new(behavior: MockBehavior.Loose);

        EventHandlerService eventHandlerService = new(
            eventHubBroker: eventHubBrokerMock.Object);

        // When
        eventHandlerService.ListenToAllEvents();

        // Then
        eventHubBrokerMock.Verify(
            expression: broker => broker.ListenToEvent<
                (int appId, Package package),
                IAppSecurityMigrationAggregationService>(
                    eventName: "content_pages_imported",
                    handler: It.IsAny<Func<
                        IAppSecurityMigrationAggregationService,
                        (int appId, Package package),
                        ValueTask>>()),
            times: Times.Once);
    }
}