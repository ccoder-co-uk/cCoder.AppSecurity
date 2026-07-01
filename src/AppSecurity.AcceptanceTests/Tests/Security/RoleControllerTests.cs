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
public sealed partial class RoleControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/Role";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededRoleContext(int AppId, Guid GuestRoleId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededRoleContext> SeedDatabase(params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("role")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Role guestRole = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("AcceptanceRole"),
            Description = "Acceptance role",
                Privs = privileges.Length == 0
                    ? "app_admin,role_create,role_update,role_delete,role_read"
                    : string.Join(',', privileges),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = guestRole.Id, UserId = "Guest" });

        return new SeededRoleContext(app.Id, guestRole.Id);
    }

    private async Task<Role> CreateRoleAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<Role>(content, JsonOptions)!;
    }

    private async Task<int> UpdateRoleAsync(Guid id, object payload)
    {
        (int statusCode, string content) = await PutRoleAsync(id, payload);
        statusCode.Should().Be((int)HttpStatusCode.OK, content);
        return statusCode;
    }

    private async Task<(int StatusCode, string Content)> PutRoleAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        return ((int)response.StatusCode, content);
    }

    private async Task<int> PatchRoleAsync(Guid id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(payload),
        };
        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteRoleAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<Role> GetRoleAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<Role>(content, JsonOptions);
    }

    private async Task Teardown(SeededRoleContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.Role.AppId == seededContext.AppId).ToArray();
        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        Role[] roles = core.Set<Role>().IgnoreQueryFilters().Where(role => role.AppId == seededContext.AppId).ToArray();
        if (roles.Length > 0)
            await core.DeleteAllAsync(roles);

        App app = core.Set<App>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.AppId);
        if (app is not null)
            await core.DeleteAsync(app);
    }

    private async Task<int> GetRoleCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<Role>> GetRolesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<Role>>(content, JsonOptions)!.Value;
    }
    private async Task<int> GetRoleStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}







