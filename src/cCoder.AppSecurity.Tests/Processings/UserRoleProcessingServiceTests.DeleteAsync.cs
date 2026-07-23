// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteWhenUserCanDeleteUserRoleForDeleteAsync()
    {
        // Given
        userRoleServiceMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => ToExternalUser(user: currentUser));

        User actor = WithPrivilege(privilege: "userrole_delete", appId: 1);

        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "page_read",
        };

        UserRole link = new()
        {
            UserId = "target-user",
            RoleId = role.Id,
            Role = role,
        };

        currentUser = actor;

        userRoleServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { link }.AsQueryable());

        userRoleServiceMock
            .Setup(expression: service => service.GetRole(roleId: role.Id))
            .Returns(value: role);

        userRoleServiceMock.Setup(expression: x => x.DeleteUserRoleAsync(deletedUserRole: link))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await userRoleProcessingService.DeleteUserRoleAsync(
deletedUserRole: new UserRole { UserId = link.UserId, RoleId = link.RoleId }
        );

        // Then
        userRoleServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);

        userRoleServiceMock.Verify(
expression: x =>
                x.DeleteUserRoleAsync(
deletedUserRole: It.Is<UserRole>(match: item =>
                        item.UserId == link.UserId && item.RoleId == link.RoleId
                    )
                ),
times: Times.Once
        );
    }

}