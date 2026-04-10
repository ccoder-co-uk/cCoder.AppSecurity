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
public sealed partial class UserControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/User";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededUserContext(int AppId, Guid GuestRoleId, Guid UserRoleId, string UserId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededUserContext> SeedDatabase(params string[] guestPrivileges)
    {
        string userId = Unique("user");
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("user")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Role guestRole = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("GuestRole"),
            Description = "Acceptance guest role",
                Privs = guestPrivileges.Length == 0
                    ? "app_admin,user_create,user_read"
                    : string.Join(',', guestPrivileges),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = guestRole.Id, UserId = "Guest" });

        await core.AddUserAsync(new User
        {
            Id = userId,
            DefaultCultureId = string.Empty,
            DisplayName = "Acceptance User",
            Email = $"{userId}@example.com",
            IsActive = true,
        });

        Role userRole = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("VisibleUserRole"),
            Description = "Acceptance user visibility role",
            Privs = "user_read",
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = userRole.Id, UserId = userId });

        return new SeededUserContext(app.Id, guestRole.Id, userRole.Id, userId);
    }

    private async Task<User> CreateUserAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<User>(content, JsonOptions)!;
    }

    private async Task<int> UpdateUserAsync(string id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchUserAsync(string id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}('{Uri.EscapeDataString(id)}')")
        {
            Content = JsonContent.Create(payload),
        };
        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteUserAsync(string id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<User> GetUserAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<User>(content, JsonOptions);
    }

    private async Task Teardown(SeededUserContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.UserId == seededContext.UserId || userRole.RoleId == seededContext.GuestRoleId || userRole.RoleId == seededContext.UserRoleId).ToArray();
        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        Role[] roles = core.Set<Role>().IgnoreQueryFilters().Where(role => role.Id == seededContext.GuestRoleId || role.Id == seededContext.UserRoleId).ToArray();
        if (roles.Length > 0)
            await core.DeleteAllAsync(roles);

        User user = core.Set<User>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.UserId);
        if (user is not null)
            await core.DeleteAsync(user);

        App app = core.Set<App>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.AppId);
        if (app is not null)
            await core.DeleteAsync(app);
    }

    private async Task<User> GetCurrentUserAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/Me()");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<User>(content, JsonOptions);
    }

    private async Task<int> GetUserCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<User>> GetUsersAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<User>>(content, JsonOptions)!.Value;
    }
    private async Task<int> GetUserStatusCodeAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')");
        return (int)response.StatusCode;
    }
}







