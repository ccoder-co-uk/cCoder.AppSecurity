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

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededUserRoleContext(int AppId, Guid GuestRoleId, Guid VisibleRoleId, string UserId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededUserRoleContext> SeedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("userrole")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Role guestRole = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("Role"),
            Description = "Acceptance role",
            Privs = "app_admin,userrole_create,userrole_delete,userrole_read",
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = guestRole.Id, UserId = "Guest" });

        string userId = Unique("user");
        await core.AddUserAsync(new User
        {
            Id = userId,
            DefaultCultureId = string.Empty,
            DisplayName = "Acceptance User",
            Email = $"{userId}@example.com",
            IsActive = true,
        });

        Role visibleRole = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("VisibleRole"),
            Description = "Acceptance visibility role",
            Privs = "user_read",
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = visibleRole.Id, UserId = userId });

        return new SeededUserRoleContext(app.Id, guestRole.Id, visibleRole.Id, userId);
    }

    private async Task<UserRole> CreateUserRoleAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<UserRole>(content, JsonOptions)!;
    }

    private async Task Teardown(SeededUserRoleContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.RoleId == seededContext.GuestRoleId || userRole.RoleId == seededContext.VisibleRoleId || userRole.UserId == seededContext.UserId).ToArray();
        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        User user = core.Set<User>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.UserId);
        if (user is not null)
            await core.DeleteAsync(user);

        Role[] roles = core.Set<Role>().IgnoreQueryFilters().Where(found => found.Id == seededContext.GuestRoleId || found.Id == seededContext.VisibleRoleId).ToArray();
        if (roles.Length > 0)
            await core.DeleteAllAsync(roles);

        App app = core.Set<App>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.AppId);
        if (app is not null)
            await core.DeleteAsync(app);
    }


    private async Task<int> GetUserRoleCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<UserRole>> GetUserRolesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<UserRole>>(content, JsonOptions)!.Value;
    }

    private async Task<UserRole> FindUserRoleAsync(string userId, Guid roleId)
    {
        IReadOnlyList<UserRole> userRoles = await GetUserRolesAsync(200);
        return userRoles.FirstOrDefault(userRole =>
            userRole.UserId == userId && userRole.RoleId == roleId
        );
    }

}







