// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUserCanCreateUserRoleForAddAsync()
    {
        // Given
        userRoleServiceMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                    throw new SecurityException(message: "Access Denied!");
            });

        userRoleServiceMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        userRoleServiceMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => ToExternalUser(user: currentUser));

        User actor = WithPrivilege(privilege: "userrole_create", appId: 1);
        User targetUser = new()
        {
            Id = "target-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Target",
            Email = "target@example.com",
            IsActive = true,
        };
        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "page_read",
            Users = [],
            App = new cCoder.Data.Models.CMS.App
            {
                Id = 1,
                Name = "App",
                Domain = "app.local",
            },
        };
        UserRole link = new() { UserId = targetUser.Id, RoleId = role.Id };
        currentUser = actor;
        userRoleServiceMock.Setup(expression: service => service.GetRole(roleId: role.Id)).Returns(value: role);
        userRoleServiceMock.Setup(expression: service => service.GetUser(userId: targetUser.Id)).Returns(value: targetUser);
        userRoleServiceMock.Setup(expression: x => x.AddUserRoleAsync(newUserRole: link)).ReturnsAsync(value: link);

        // When
        UserRole result = await userRoleProcessingService.AddUserRoleAsync(newUserRole: link);

        // Then
        Assert.Same(expected: link, actual: result);
        userRoleServiceMock.Verify(expression: x => x.AddUserRoleAsync(newUserRole: link), times: Times.Once);
    }

    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAppAdminAssignsAppAdminRoleForAddAsync()
    {
        // Given
        userRoleServiceMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                    throw new SecurityException(message: "Access Denied!");
            });

        userRoleServiceMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int?>()))
            .Returns(valueFunction: (int? appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        userRoleServiceMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => ToExternalUser(user: currentUser));

        User actor = WithPrivilege(privilege: "userrole_create,app_admin", appId: 1);
        User targetUser = new()
        {
            Id = "target-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Target",
            Email = "target@example.com",
            IsActive = true,
        };
        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Administrators",
            Privs = "app_admin,page_read",
            Users = [],
            App = new cCoder.Data.Models.CMS.App
            {
                Id = 1,
                Name = "App",
                Domain = "app.local",
            },
        };
        UserRole link = new() { UserId = targetUser.Id, RoleId = role.Id };
        currentUser = actor;
        userRoleServiceMock.Setup(expression: service => service.GetRole(roleId: role.Id)).Returns(value: role);
        userRoleServiceMock.Setup(expression: service => service.GetUser(userId: targetUser.Id)).Returns(value: targetUser);
        userRoleServiceMock.Setup(expression: x => x.AddUserRoleAsync(newUserRole: link)).ReturnsAsync(value: link);

        // When
        UserRole result = await userRoleProcessingService.AddUserRoleAsync(newUserRole: link);

        // Then
        Assert.Same(expected: link, actual: result);
        userRoleServiceMock.Verify(expression: x => x.AddUserRoleAsync(newUserRole: link), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        userRoleServiceMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                    throw new SecurityException(message: "Access Denied!");
            });

        userRoleServiceMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        userRoleServiceMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => ToExternalUser(user: currentUser));

        User targetUser = new()
        {
            Id = "target-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Target",
            Email = "target@example.com",
            IsActive = true,
        };
        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "page_read",
            Users = [],
            App = new cCoder.Data.Models.CMS.App
            {
                Id = 1,
                Name = "App",
                Domain = "app.local",
            },
        };
        userRoleServiceMock.Setup(expression: service => service.GetRole(roleId: role.Id)).Returns(value: role);
        userRoleServiceMock.Setup(expression: service => service.GetUser(userId: targetUser.Id)).Returns(value: targetUser);

        // When
        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await userRoleProcessingService.AddUserRoleAsync(
newUserRole:                 new UserRole { UserId = targetUser.Id, RoleId = role.Id }
            )
        );

        // Then
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenNonAdminAssignsAppAdminRoleForAddAsync()
    {
        // Given
        userRoleServiceMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                    throw new SecurityException(message: "Access Denied!");
            });

        userRoleServiceMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int?>()))
            .Returns(valueFunction: (int? appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        userRoleServiceMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => ToExternalUser(user: currentUser));

        User actor = WithPrivilege(privilege: "userrole_create", appId: 1);
        User targetUser = new()
        {
            Id = "target-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Target",
            Email = "target@example.com",
            IsActive = true,
        };
        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Administrators",
            Privs = "app_admin,page_read",
            Users = [],
            App = new cCoder.Data.Models.CMS.App
            {
                Id = 1,
                Name = "App",
                Domain = "app.local",
            },
        };
        currentUser = actor;
        userRoleServiceMock.Setup(expression: service => service.GetRole(roleId: role.Id)).Returns(value: role);
        userRoleServiceMock.Setup(expression: service => service.GetUser(userId: targetUser.Id)).Returns(value: targetUser);

        // When
        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await userRoleProcessingService.AddUserRoleAsync(
newUserRole:                 new UserRole { UserId = targetUser.Id, RoleId = role.Id }
            )
        );

        // Then
        userRoleServiceMock.Verify(
expression:             x => x.AddUserRoleAsync(newUserRole: It.IsAny<UserRole>()),
times:             Times.Never
        );
    }

}