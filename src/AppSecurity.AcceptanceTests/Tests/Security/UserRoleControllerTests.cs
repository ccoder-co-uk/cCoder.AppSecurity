// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.Security;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class UserRoleControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/UserRole";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededUserRoleContext(
        int AppId,
        Guid GuestRoleId,
        Guid VisibleRoleId,
        Guid BasicRoleId,
        string UserId,
        string HiddenUserId
    );
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededUserRoleContext> SeedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "userrole")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role guestRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "Role"),
            Description = "Acceptance role",
            Privs = "app_admin,userrole_create,userrole_delete,userrole_read,page_read",
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = guestRole.Id, UserId = "Guest" });

        string userId = Unique(prefix: "user");

        await core.AddUserAsync(user: new User
        {
            Id = userId,
            DefaultCultureId = string.Empty,
            DisplayName = "Acceptance User",
            Email = $"{userId}@example.com",
            IsActive = true,
        });

        Role visibleRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "VisibleRole"),
            Description = "Acceptance visibility role",
            Privs = "user_read",
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = visibleRole.Id, UserId = userId });

        Role basicRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "BasicRole"),
            Description = "Acceptance basic role",
            Privs = "page_read",
        });

        string hiddenUserId = Unique(prefix: "hidden-user");

        await core.AddUserAsync(user: new User
        {
            Id = hiddenUserId,
            DefaultCultureId = string.Empty,
            DisplayName = "Hidden User",
            Email = $"{hiddenUserId}@example.com",
            IsActive = true,
        });

        return new SeededUserRoleContext(
AppId: app.Id,
GuestRoleId: guestRole.Id,
VisibleRoleId: visibleRole.Id,
BasicRoleId: basicRole.Id,
UserId: userId,
HiddenUserId: hiddenUserId
        );
    }

    private sealed record RestrictedUserRoleContext(int AppId, Guid OperatorRoleId, Guid TargetAdminRoleId, string UserId);

    private async Task<RestrictedUserRoleContext> SeedRestrictedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "RestrictedApp"),
            Domain = $"{Unique(prefix: "restricted-userrole")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role operatorRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "OperatorRole"),
            Description = "Can manage user-role links but is not app admin",
            Privs = "userrole_create,userrole_read",
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = operatorRole.Id, UserId = "Guest" });

        Role targetAdminRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "AdminRole"),
            Description = "Protected admin role",
            Privs = "app_admin,page_read",
        });

        string userId = Unique(prefix: "restricted-user");

        await core.AddUserAsync(user: new User
        {
            Id = userId,
            DefaultCultureId = string.Empty,
            DisplayName = "Restricted User",
            Email = $"{userId}@example.com",
            IsActive = true,
        });

        return new RestrictedUserRoleContext(AppId: app.Id, OperatorRoleId: operatorRole.Id, TargetAdminRoleId: targetAdminRole.Id, UserId: userId);
    }

    private async Task<UserRole> CreateUserRoleAsync(object payload)
    {
        (int statusCode, string content) = await PostUserRoleAsync(payload: payload);

        statusCode.Should()
            .Be(expected: (int)HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<UserRole>(json: content, options: JsonOptions)!;
    }

    private async Task<(int StatusCode, string Content)> PostUserRoleAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();
        return ((int)response.StatusCode, content);
    }

    private async Task<int> DeleteUserRoleAsync(Guid roleId, string userId)
    {

        using HttpResponseMessage response = await Client.DeleteAsync(
requestUri: $"{BaseUrl}(RoleId={roleId},UserId='{Uri.EscapeDataString(stringToEscape: userId)}')"
        );

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task Teardown(SeededUserRoleContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole =>
            userRole.RoleId == seededContext.GuestRoleId
            || userRole.RoleId == seededContext.VisibleRoleId
            || userRole.RoleId == seededContext.BasicRoleId
            || userRole.UserId == seededContext.UserId
            || userRole.UserId == seededContext.HiddenUserId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        User[] users = core.Set<User>()
            .IgnoreQueryFilters()
            .Where(predicate: found =>
            found.Id == seededContext.UserId || found.Id == seededContext.HiddenUserId)
            .ToArray();

        foreach (User user in users)
        {
            await core.DeleteAsync(user: user);
        }

        Role[] roles = core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: found =>
            found.Id == seededContext.GuestRoleId
            || found.Id == seededContext.VisibleRoleId
            || found.Id == seededContext.BasicRoleId)
            .ToArray();

        if (roles.Length > 0)
        {
            await core.DeleteAllAsync(roles: roles);
        }

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.AppId);

        if (app is not null)
        {
            await core.DeleteAsync(app: app);
        }
    }

    private async Task Teardown(RestrictedUserRoleContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole =>
            userRole.RoleId == seededContext.OperatorRoleId
            || userRole.RoleId == seededContext.TargetAdminRoleId
            || userRole.UserId == seededContext.UserId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        User user = core.Set<User>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.UserId);

        if (user is not null)
        {
            await core.DeleteAsync(user: user);
        }

        Role[] roles = core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: found =>
            found.Id == seededContext.OperatorRoleId
            || found.Id == seededContext.TargetAdminRoleId)
            .ToArray();

        if (roles.Length > 0)
        {
            await core.DeleteAllAsync(roles: roles);
        }

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.AppId);

        if (app is not null)
        {
            await core.DeleteAsync(app: app);
        }
    }

    private async Task<int> GetUserRoleCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<UserRole>> GetUserRolesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<UserRole>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<UserRole> FindUserRoleAsync(string userId, Guid roleId)
    {
        IReadOnlyList<UserRole> userRoles = await GetUserRolesAsync(top: 200);

        return userRoles.FirstOrDefault(predicate: userRole =>
            userRole.UserId == userId && userRole.RoleId == roleId
        );
    }

}