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
    public async Task ShouldUpdateUserWithoutAuthorizationOnUpdateFromAccountEventAsync()
    {
        // Given
        User user = CreateRandomUser();

        user.Roles =
        [
            new UserRole
            {
                RoleId = Guid.NewGuid(),
                UserId = user.Id,
                Role = new Role
                {
                    Id = Guid.NewGuid(),
                    AppId = 7,
                    Name = "Administrator",
                    Privs = "page_read"
                }
            }
        ];

        userBrokerMock
            .Setup(expression: broker => broker.UpdateUserAsync(
                entity: It.IsAny<User>()))
            .ReturnsAsync(valueFunction: (User value) => value);

        // When
        User result = await userService.UpdateUserFromAccountEventAsync(
            updatedUser: user);

        // Then
        result
            .Should()
            .BeSameAs(expected: user);

        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}