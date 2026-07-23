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
public sealed partial class RoleControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/Role";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed class SeededRoleContext
    {
        public required int AppId { get; init; }

        public required Guid GuestRoleId { get; init; }
    }

    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededRoleContext> SeedDatabase(params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "role")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role guestRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "AcceptanceRole"),
            Description = "Acceptance role",
            Privs = privileges.Length == 0
                    ? "app_admin,role_create,role_update,role_delete,role_read"
                    : string.Join(separator: ',', value: privileges),
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = guestRole.Id, UserId = "Guest" });

        return new SeededRoleContext
        {
            AppId = app.Id,
            GuestRoleId = guestRole.Id,
        };
    }

    private async Task<Role> CreateRoleAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<Role>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateRoleAsync(Guid id, object payload)
    {
        var result = await PutRoleAsync(id: id, payload: payload);
        int statusCode = result.StatusCode;
        string content = result.Content;

        statusCode.Should()
            .Be(expected: (int)HttpStatusCode.OK, because: content);

        return statusCode;
    }

    private async Task<(int StatusCode, string Content)> PutRoleAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();
        return ((int)response.StatusCode, content);
    }

    private async Task<int> PatchRoleAsync(Guid id, object payload)
    {

        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteRoleAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<Role> GetRoleAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");

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

        return JsonSerializer.Deserialize<Role>(json: content, options: JsonOptions);
    }

    private async Task Teardown(SeededRoleContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        UserRole[] userRoles = core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.Role.AppId == seededContext.AppId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        Role[] roles = core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.AppId == seededContext.AppId)
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

    private async Task<int> GetRoleCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<Role>> GetRolesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<Role>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<int> GetRoleStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}