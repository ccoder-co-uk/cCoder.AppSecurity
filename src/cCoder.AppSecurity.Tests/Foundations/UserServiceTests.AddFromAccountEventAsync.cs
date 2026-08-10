// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserServiceTests
{
    [Fact]
    public async Task ShouldAddUserWithoutRequestAuthorizationForAddFromAccountEventAsync()
    {
        // Given
        User user = CreateRandomUser();

        userBrokerMock
            .Setup(expression: broker => broker.AddUserAsync(
                entity: It.IsAny<User>()))
            .ReturnsAsync(valueFunction: (User value) => value);

        // When
        User result = await userService
            .AddUserFromAccountEventAsync(newUser: user);

        // Then
        result.Should()
            .BeSameAs(expected: user);

        userBrokerMock.Verify(
            expression: broker => broker.AddUserAsync(
                entity: It.IsAny<User>()),
            times: Times.Once);

        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}