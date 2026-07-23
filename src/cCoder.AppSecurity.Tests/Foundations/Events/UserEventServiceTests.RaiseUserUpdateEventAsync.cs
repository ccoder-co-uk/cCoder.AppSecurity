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

public partial class UserEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseUserUpdateEventAsync()
    {
        // Given
        User entity = new();
        EventMessage<cCoder.Data.Models.Security.User> actualMessage = null;

        userEventBrokerMock
            .Setup(expression: x => x.RaiseUserUpdateEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Security.User>>()))
            .Callback<EventMessage<cCoder.Data.Models.Security.User>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserUpdateEventAsync(entity: entity);

        // Then
        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        userEventBrokerMock.Verify(
expression: x => x.RaiseUserUpdateEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Security.User>>()),
times: Times.Once
        );

        userEventBrokerMock.VerifyNoOtherCalls();
    }

}