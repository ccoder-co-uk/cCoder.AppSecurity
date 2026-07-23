// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private async Task CreatePrivilegeAsync(object payload)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        await core.AddPrivilegeAsync(privilege: new Privilege
        {
            Id = (string)payload.GetType()
            .GetProperty(name: "id")!.GetValue(obj: payload)!,
            Type = (string)payload.GetType()
            .GetProperty(name: "type")!.GetValue(obj: payload)!,
            Operation = (string)payload.GetType()
            .GetProperty(name: "operation")!.GetValue(obj: payload)!,
            Description = (string)payload.GetType()
            .GetProperty(name: "description")!.GetValue(obj: payload)!,
            PortalAdminsOnly = (bool)payload.GetType()
            .GetProperty(name: "portalAdminsOnly")!.GetValue(obj: payload)!,
        });
    }

    private async Task DeletePrivilegeAsync(string id)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Privilege privilege = core.Set<Privilege>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == id);

        if (privilege is not null)
        {
            await core.DeleteAsync(privilege: privilege);
        }
    }

    private async Task<Privilege> GetPrivilegeAsync(string id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/{Uri.EscapeDataString(stringToEscape: id)}");

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

        return JsonSerializer.Deserialize<Privilege>(json: content, options: JsonOptions);
    }

    private async Task<int> GetPrivilegeStatusCodeAsync(string relativeUrl)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: relativeUrl);
        return (int)response.StatusCode;
    }
}