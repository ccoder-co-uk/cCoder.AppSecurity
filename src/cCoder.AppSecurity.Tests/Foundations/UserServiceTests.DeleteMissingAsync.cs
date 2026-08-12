// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserServiceTests
{
    [Fact]
    public async Task ShouldReturnWithoutDeletingWhenUserDoesNotExistAsync()
    {
        // Given
        userBrokerMock
            .Setup(expression: broker => broker.GetAllUsers(ignoreFilters: false))
            .Returns(value: Array.Empty<User>()
                .AsQueryable());

        userBrokerMock
            .Setup(expression: broker => broker.GetAllUsers(ignoreFilters: true))
            .Returns(value: Array.Empty<User>()
                .AsQueryable());

        // When
        await userService.DeleteAsync(userId: "missing");

        // Then
        userBrokerMock.Verify(
            expression: broker => broker.GetAllUsers(ignoreFilters: false),
            times: Times.Once);

        userBrokerMock.Verify(
            expression: broker => broker.GetAllUsers(ignoreFilters: true),
            times: Times.Once);

        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}