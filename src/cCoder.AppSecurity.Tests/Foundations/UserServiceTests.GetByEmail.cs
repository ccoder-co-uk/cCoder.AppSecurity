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
    public void ShouldMapNestedRoleWhenGetByEmailSucceeds()
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
                    Description = "Administrator role",
                    Privs = "page_read"
                }
            }
        ];

        cCoder.Data.Models.Security.User externalUser =
            ToExternalUser(item: user);

        userBrokerMock
            .Setup(expression: broker => broker.GetUserByEmail(
                email: user.Email,
                ignoreFilters: true))
            .Returns(value: externalUser);

        // When
        User result = userService.GetByEmail(
            email: user.Email,
            ignoreFilters: true);

        // Then
        result
            .Should()
            .BeEquivalentTo(expectation: user);
    }

    [Fact]
    public void ShouldReturnNullWhenGetByEmailCannotFindUser()
    {
        // Given
        userBrokerMock
            .Setup(expression: broker => broker.GetUserByEmail(
                email: "missing@example.com",
                ignoreFilters: false))
            .Returns(value: null);

        // When
        User result = userService.GetByEmail(email: "missing@example.com");

        // Then
        result
            .Should()
            .BeNull();
    }
}