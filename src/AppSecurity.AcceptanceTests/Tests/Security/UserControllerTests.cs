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

[Collection(name: WebAcceptanceCollection.Name)]
public sealed partial class UserControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/User";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededUserContext(int AppId, Guid GuestRoleId, Guid UserRoleId, string UserId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededUserContext> SeedDatabase(params string[] guestPrivileges)
    {
        string userId = Unique(prefix: "user");
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "user")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role guestRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "GuestRole"),
            Description = "Acceptance guest role",
            Privs = guestPrivileges.Length == 0
                    ? "app_admin,user_create,user_read"
                    : string.Join(separator: ',', value: guestPrivileges),
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = guestRole.Id, UserId = "Guest" });

        User user = new()
        {
            Id = userId,
            DefaultCultureId = string.Empty,
            DisplayName = "Acceptance User",
            Email = $"{userId}@example.com",
            IsActive = true,
        };

        await core.AddUserAsync(user: user);

        Role userRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "VisibleUserRole"),
            Description = "Acceptance user visibility role",
            Privs = "user_read",
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = userRole.Id, UserId = userId });

        return new SeededUserContext(AppId: app.Id, GuestRoleId: guestRole.Id, UserRoleId: userRole.Id, UserId: userId);
    }

    private async Task<User> CreateUserAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<User>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateUserAsync(string id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchUserAsync(string id, object payload)
    {

        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteUserAsync(string id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<User> GetUserAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        if (content.Contains(value: "\"value\":[]", comparisonType: StringComparison.Ordinal))
        {
            return null;
        }

        return JsonSerializer.Deserialize<User>(json: content, options: JsonOptions);
    }

    private async Task Teardown(SeededUserContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.UserId == seededContext.UserId || userRole.RoleId == seededContext.GuestRoleId || userRole.RoleId == seededContext.UserRoleId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        Role[] roles = core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.Id == seededContext.GuestRoleId || role.Id == seededContext.UserRoleId)
            .ToArray();

        if (roles.Length > 0)
        {
            await core.DeleteAllAsync(roles: roles);
        }

        User user = core.Set<User>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.UserId);

        if (user is not null)
        {
            await core.DeleteAsync(user: user);
        }

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.AppId);

        if (app is not null)
        {
            await core.DeleteAsync(app: app);
        }
    }

    private async Task<User> GetCurrentUserAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/Me()");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<User>(json: content, options: JsonOptions);
    }

    private async Task<int> GetUserCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<User>> GetUsersAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<User>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<int> GetUserStatusCodeAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')");
        return (int)response.StatusCode;
    }
}