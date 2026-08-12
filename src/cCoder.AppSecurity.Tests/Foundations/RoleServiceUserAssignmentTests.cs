// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Storages;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class RoleServiceUserAssignmentTests
{
    [Fact]
    public async Task ShouldMapAndAddOnlyNewUserAssignmentsWhenAddingRoleAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        var role = new Role
        {
            Id = roleId,
            AppId = 7,
            Name = "Users",
            Privs = string.Empty,
            Pages = [new PageRole { PageId = 11, RoleId = roleId }],
            Folders = [new FolderRole { FolderId = Guid.NewGuid(), RoleId = roleId }],
            Users =
            [
                new UserRole
                {
                    UserId = "existing-user",
                    User = new User
                    {
                        Id = "existing-user",
                        DisplayName = "Existing",
                        Email = "existing@example.com"
                    }
                },
                new UserRole
                {
                    UserId = "new-user",
                    User = new User
                    {
                        Id = "new-user",
                        DisplayName = "New",
                        Email = "new@example.com"
                    }
                }
            ]
        };

        Mock<IRoleBroker> roleBrokerMock = new();
        Mock<IUserRoleBroker> userRoleBrokerMock = new();
        Mock<IAuthorizationBroker> authorizationBrokerMock = new();

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: true))
            .Returns(value: Array.Empty<Role>()
                .AsQueryable());

        roleBrokerMock
            .Setup(expression: broker => broker.AddRoleAsync(entity: It.IsAny<Role>()))
            .ReturnsAsync(valueFunction: (Role submitted) => submitted);

        userRoleBrokerMock
            .Setup(expression: broker => broker.GetAllUserRoles(ignoreFilters: true))
            .Returns(value: new[]
            {
                new UserRole { RoleId = roleId, UserId = "existing-user" }
            }.AsQueryable());

        userRoleBrokerMock
            .Setup(expression: broker => broker.AddUserRoleAsync(
                entity: It.Is<UserRole>(match: assignment =>
                    assignment.UserId == "new-user"
                    && assignment.User.DisplayName == "New")))
            .ReturnsAsync(value: new UserRole());

        var service = new RoleService(
            roleBroker: roleBrokerMock.Object,
            userRoleBroker: userRoleBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);

        // When
        Role result = await service.AddRoleAsync(newRole: role);

        // Then
        result
            .Should()
            .BeSameAs(expected: role);

        userRoleBrokerMock.Verify(
            expression: broker => broker.AddUserRoleAsync(entity: It.IsAny<UserRole>()),
            times: Times.Once);
    }
}