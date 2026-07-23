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
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));

        User actor = WithPrivilege("userrole_create", 1);
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
        roleServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { role }.AsQueryable());
        userServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { targetUser }.AsQueryable());
        userRoleServiceMock.Setup(x => x.AddAsync(link)).ReturnsAsync(link);

        // When
        UserRole result = await userRoleProcessingService.AddAsync(link);

        // Then
        Assert.Same(link, result);
        userRoleServiceMock.Verify(x => x.AddAsync(link), Times.Once);
    }

    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAppAdminAssignsAppAdminRoleForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int?>()))
            .Returns((int? appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));

        User actor = WithPrivilege("userrole_create,app_admin", 1);
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
        roleServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { role }.AsQueryable());
        userServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { targetUser }.AsQueryable());
        userRoleServiceMock.Setup(x => x.AddAsync(link)).ReturnsAsync(link);

        // When
        UserRole result = await userRoleProcessingService.AddAsync(link);

        // Then
        Assert.Same(link, result);
        userRoleServiceMock.Verify(x => x.AddAsync(link), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));

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
        roleServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { role }.AsQueryable());
        userServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { targetUser }.AsQueryable());

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await userRoleProcessingService.AddAsync(
                new UserRole { UserId = targetUser.Id, RoleId = role.Id }
            )
        );

        // Then
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenNonAdminAssignsAppAdminRoleForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int?>()))
            .Returns((int? appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));

        User actor = WithPrivilege("userrole_create", 1);
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
        roleServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { role }.AsQueryable());
        userServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { targetUser }.AsQueryable());

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await userRoleProcessingService.AddAsync(
                new UserRole { UserId = targetUser.Id, RoleId = role.Id }
            )
        );

        // Then
        userRoleServiceMock.Verify(
            x => x.AddAsync(It.IsAny<UserRole>()),
            Times.Never
        );
    }

}