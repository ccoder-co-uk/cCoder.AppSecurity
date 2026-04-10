using System.Net;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.Security;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class PrivilegeControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/Privilege";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private async Task CreatePrivilegeAsync(object payload)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        await core.AddPrivilegeAsync(new Privilege
        {
            Id = (string)payload.GetType().GetProperty("id")!.GetValue(payload)!,
            Type = (string)payload.GetType().GetProperty("type")!.GetValue(payload)!,
            Operation = (string)payload.GetType().GetProperty("operation")!.GetValue(payload)!,
            Description = (string)payload.GetType().GetProperty("description")!.GetValue(payload)!,
            PortalAdminsOnly = (bool)payload.GetType().GetProperty("portalAdminsOnly")!.GetValue(payload)!,
        });
    }

    private async Task DeletePrivilegeAsync(string id)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();
        Privilege privilege = core.Set<Privilege>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == id);
        if (privilege is not null)
            await core.DeleteAsync(privilege);
    }

    private async Task<Privilege> GetPrivilegeAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(id)}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<Privilege>(content, JsonOptions);
    }

    private async Task<int> GetPrivilegeStatusCodeAsync(string relativeUrl)
    {
        using HttpResponseMessage response = await Client.GetAsync(relativeUrl);
        return (int)response.StatusCode;
    }
}







