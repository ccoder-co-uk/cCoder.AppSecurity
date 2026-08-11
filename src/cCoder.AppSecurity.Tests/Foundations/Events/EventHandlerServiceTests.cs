// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Services.Foundations.Events;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations.Events;

public sealed partial class EventHandlerServiceTests
{
    [Fact]
    public void ShouldNotRegisterPackageImportListeners()
    {
        // Given
        Mock<IEventHubBroker> eventHubBrokerMock = new(
            behavior: MockBehavior.Loose);

        EventHandlerService eventHandlerService = new(
            eventHubBroker: eventHubBrokerMock.Object);

        // When
        eventHandlerService.ListenToAllEvents();

        // Then
        Assert.DoesNotContain(
            collection: eventHubBrokerMock.Invocations,
            filter: invocation => invocation.Arguments.Count > 0
                && invocation.Arguments[0] as string is
                    "package_import" or "content_pages_imported");
    }
}