// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Models;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class AppSecurityPackageTests
{
    [Fact]
    public async Task ShouldRoundTripRolesIdempotentlyWhenUsingPackageManagerAsync()
    {
        // Given
        (int sourceAppId, int destinationAppId) = await SeedRoleRoundTripAppsAsync();

        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        IAppSecurityPackageManager packageManager = scope.ServiceProvider
            .GetRequiredService<IAppSecurityPackageManager>();

        AppSecurityPackage exportedPackage = packageManager.ExportPackage(
            appId: sourceAppId,
            packageName: "Roles");

        string[] expectedRoles = NormalizeRoles(package: exportedPackage);

        // When
        await packageManager.ImportPackageAsync(
            appId: destinationAppId,
            appSecurityPackage: exportedPackage);

        AppSecurityPackage firstDestinationExport = packageManager.ExportPackage(
            appId: destinationAppId,
            packageName: "Roles");

        await packageManager.ImportPackageAsync(
            appId: destinationAppId,
            appSecurityPackage: exportedPackage);

        AppSecurityPackage secondDestinationExport = packageManager.ExportPackage(
            appId: destinationAppId,
            packageName: "Roles");

        // Then
        NormalizeRoles(package: firstDestinationExport)
            .Should()
            .Equal(expected: expectedRoles);

        NormalizeRoles(package: secondDestinationExport)
            .Should()
            .Equal(expected: expectedRoles);

        using var core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        Role[] destinationRoles = await core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.AppId == destinationAppId)
            .ToArrayAsync();

        destinationRoles.Should()
            .HaveCount(expected: expectedRoles.Length);

        destinationRoles
            .GroupBy(
                keySelector: role => role.Name,
                comparer: StringComparer.OrdinalIgnoreCase)
            .Should()
            .OnlyContain(predicate: group => group.Count() == 1);
    }

    private async Task<(int SourceAppId, int DestinationAppId)>
        SeedRoleRoundTripAppsAsync()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

        App sourceApp = await core.AddAppAsync(app: CreateApp(name: "Role source"));
        App destinationApp = await core.AddAppAsync(app: CreateApp(name: "Role destination"));

        await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = sourceApp.Id,
            Name = "Analysts",
            Privs = "role_read,user_read",
        });

        await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = sourceApp.Id,
            Name = "Publishers",
            Privs = "page_read,page_update",
        });

        await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = destinationApp.Id,
            Name = "Analysts",
            Privs = "stale_privilege",
        });

        return (sourceApp.Id, destinationApp.Id);
    }

    private static App CreateApp(string name) =>
        new()
        {
            Name = name,
            Domain = $"{Guid.NewGuid():N}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = $"tenant-{Guid.NewGuid():N}",
            ConfigJson = "{}",
        };

    private static string[] NormalizeRoles(AppSecurityPackage package) =>
        [.. package.Items
            .Where(predicate: item =>
                item.Type is "Core/Role" or "AppSecurity/Role")
            .SelectMany(selector: item => ReadRoles(data: item.Data))
            .OrderBy(keySelector: role => role, comparer: StringComparer.OrdinalIgnoreCase)];

    private static IEnumerable<string> ReadRoles(string data)
    {
        using JsonDocument document = JsonDocument.Parse(json: data);

        IEnumerable<JsonElement> roles = document.RootElement.ValueKind == JsonValueKind.Array
            ? document.RootElement.EnumerateArray()
            : [document.RootElement];

        return roles
            .Select(selector: NormalizeRole)
            .ToArray();
    }

    private static string NormalizeRole(JsonElement role)
    {
        string name = role
            .GetProperty(propertyName: "Name")
            .GetString();

        string privileges = role
            .GetProperty(propertyName: "Privs")
            .GetString();

        return $"{name}|{privileges}";
    }
}