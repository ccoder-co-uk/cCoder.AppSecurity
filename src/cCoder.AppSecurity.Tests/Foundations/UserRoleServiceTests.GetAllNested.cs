// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserRoleServiceTests
{
    [Fact]
    public void ShouldMapNestedUserAndRoleWhenGetAll()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        Culture culture = new() { Id = "en-GB" };

        DataRole role = new()
        {
            Id = roleId,
            AppId = 7,
            Name = "Users",
            Description = "Application users",
            Privs = "page_read"
        };

        DataUser user = new()
        {
            Id = "user-one",
            DefaultCultureId = culture.Id,
            DisplayName = "User One",
            Email = "user@example.test",
            IsActive = true,
            DefaultCulture = culture
        };

        DataUserRole userRole = new()
        {
            RoleId = roleId,
            UserId = user.Id,
            Role = role,
            User = user
        };

        userRoleBrokerMock
            .Setup(expression: broker => broker.GetAllUserRoles(
                ignoreFilters: false))
            .Returns(value: new[] { userRole }.AsQueryable());

        // When
        IQueryable<UserRole> result = userRoleService.GetAll();

        // Then
        UserRole mapped = result.Single();

        mapped.User
            .Should()
            .BeEquivalentTo(expectation: user);

        mapped.Role
            .Should()
            .BeEquivalentTo(expectation: role);
    }

    [Fact]
    public async Task ShouldAddWithoutRoleLookupWhenAppIsUnknownAsync()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        userRoleBrokerMock
            .Setup(expression: broker => broker.GetAppId(
                entity: It.Is<DataUserRole>(match: _ => true)))
            .Returns(value: null);

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: null,
                privilege: "UserRole_create"));

        userRoleBrokerMock
            .Setup(expression: broker => broker.AddUserRoleAsync(
                entity: It.Is<DataUserRole>(match: _ => true)))
            .ReturnsAsync(value: userRole);

        // When
        UserRole result = await userRoleService.AddUserRoleAsync(
            newUserRole: userRole);

        // Then
        result
            .Should()
            .BeSameAs(expected: userRole);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}