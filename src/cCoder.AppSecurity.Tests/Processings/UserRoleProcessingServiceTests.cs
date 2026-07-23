// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using Moq;
namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleProcessingServiceTests
{
    private User currentUser = WithoutPrivileges();
    private readonly Mock<IUserRoleFoundationService> userRoleServiceMock = new();
    private readonly UserRoleProcessingService userRoleProcessingService;

    public UserRoleProcessingServiceTests()
    {

        userRoleProcessingService = new UserRoleProcessingService(
            service: userRoleServiceMock.Object);
    }

    private static User WithPrivilege(string privilege, int appId = 1)
    {

        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = "Test Role",
            Privs = privilege.ToLowerInvariant(),
            App = new App { Id = appId, Name = "App", Domain = "app.local" },
            Users = [],
            Pages = [],
            Folders = [],
        };

        User user = new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
        };

        UserRole userRole = new()
        {
            Role = role,
            RoleId = role.Id,
            User = user,
            UserId = user.Id,
        };

        user.Roles = [userRole];
        role.Users = [userRole];
        return user;
    }

    private static User WithoutPrivileges() =>
        new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = [],
        };

    private static cCoder.Data.Models.Security.User ToExternalUser(User user) =>
        new()
        {
            Id = user.Id,
            DefaultCultureId = user.DefaultCultureId,
            DisplayName = user.DisplayName,
            Email = user.Email,
            IsActive = user.IsActive,
        };
}