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

public partial class UserRoleEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseUserRoleDeleteEventAsync()
    {
        // Given
        UserRole entity = new();
        EventMessage<cCoder.Data.Models.Security.UserRole> actualMessage = null;

        userRoleEventBrokerMock
            .Setup(expression: x => x.RaiseUserRoleDeleteEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Security.UserRole>>()))
            .Callback<EventMessage<cCoder.Data.Models.Security.UserRole>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserRoleDeleteEventAsync(entity: entity);

        // Then
        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        userRoleEventBrokerMock.Verify(
expression: x => x.RaiseUserRoleDeleteEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Security.UserRole>>()),
times: Times.Once
        );

        userRoleEventBrokerMock.VerifyNoOtherCalls();
    }

}