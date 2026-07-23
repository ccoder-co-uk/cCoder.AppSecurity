// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class PrivilegeEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePrivilegeAddEventAsync()
    {
        // Given
        Privilege entity = new();
        EventMessage<cCoder.Data.Models.Security.Privilege> actualMessage = null;

        privilegeEventBrokerMock
            .Setup(expression: x => x.RaisePrivilegeAddEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Security.Privilege>>()))
            .Callback<EventMessage<cCoder.Data.Models.Security.Privilege>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePrivilegeAddEventAsync(entity: entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(expectation: entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(expected: CurrentUserId);
        privilegeEventBrokerMock.Verify(
expression:             x => x.RaisePrivilegeAddEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Security.Privilege>>()),
times:             Times.Once
        );
        privilegeEventBrokerMock.VerifyNoOtherCalls();
    }

}