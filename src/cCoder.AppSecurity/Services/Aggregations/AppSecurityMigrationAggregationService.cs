// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;

namespace cCoder.AppSecurity.Services.Aggregations;

internal sealed partial class AppSecurityMigrationAggregationService(
    IRoleProcessingService roleProcessingService,
    IJsonProcessingService jsonProcessingService
) : IAppSecurityMigrationAggregationService
{
    public ValueTask ImportPackageAppSecurityPackageAsync(int appId, AppSecurityPackage package) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateImportPackageAppSecurityPackage(
                appId: appId,
                package: package);

            if (package.Items is null || package.Items.Count == 0)
            {
                return;
            }

            Role[] roles = package.Items
                .Where(predicate: item =>
                    item.Type is "Core/Role" or "AppSecurity/Role")
                .SelectMany(selector: item => item.Data.StartsWith(value: "{")
                    ? [jsonProcessingService.ParseJson<Role>(json: item.Data)]
                    : jsonProcessingService.ParseJson<Role[]>(json: item.Data))
                .ToArray();

            PageRoleInfo[] pageRoleInfos = package.Items
                .Where(predicate: item => item.Type == "ContentManagement/PageRole")
                .SelectMany(selector: item => item.Data.StartsWith(value: "{")
                    ? [jsonProcessingService.ParseJson<PageRoleInfo>(json: item.Data)]
                    : jsonProcessingService.ParseJson<PageRoleInfo[]>(json: item.Data))
                .ToArray();

            if (roles.Length == 0 && pageRoleInfos.Length > 0)
            {
                string[] roleNames = pageRoleInfos
                    .Select(selector: pageRole => pageRole.Role)
                    .Distinct(comparer: StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                roles = roleProcessingService.GetAll(ignoreFilters: true)
                    .Where(predicate: role =>
                        role.AppId == appId
                        && roleNames.Contains(value: role.Name))
                    .ToArray();
            }

            AttachPageRoles(
                appId: appId,
                roles: roles,
                pageRoleInfos: pageRoleInfos);

            await ImportRolesAsync(
                appId: appId,
                roles: roles);

        });

    public AppSecurityPackage ExportPackage(int appId, string packageName) =>
        TryCatch(operation: AppSecurityPackage () =>
        {
            ValidateExportPackage(
                appId: appId,
                packageName: packageName);

            return packageName == "Roles"
            ? new AppSecurityPackage
            {
                Name = "Roles",
                Items =
                [
                    new AppSecurityPackageItem
                    {
                        Type = "Core/Role",
                        Data = jsonProcessingService.Serialize(
                            value: roleProcessingService
                            .GetAll(ignoreFilters: true)
                            .Where(predicate: role => role.AppId == appId)
                            .Select(selector: role => new { role.Name, role.Privs })
                            .ToArray()),
                    },
                ],
            }
            : new AppSecurityPackage
            {
                Name = packageName,
                Items = [],
            };
        });

    private async ValueTask ImportRolesAsync(
        int appId,
        IEnumerable<Role> roles)
    {
        var dbVersions = roleProcessingService
            .GetAll(ignoreFilters: true)
            .Where(predicate: role => role.AppId == appId)
            .Select(selector: role => new
            {
                role.Id,
                role.Name,
            })
            .ToArray();

        foreach (Role role in roles)
        {
            role.AppId = appId;

            role.Id = dbVersions
                .FirstOrDefault(predicate: existing => existing.Name == role.Name)
                ?.Id ?? Guid.Empty;

            if (role.Id == Guid.Empty)
            {
                await roleProcessingService.AddValidatedRoleAsync(
                    entity: role);
            }
            else
            {
                await roleProcessingService.UpdateValidatedRoleAsync(
                    entity: role);
            }
        }
    }

    private static void AttachPageRoles(
        int appId,
        IEnumerable<Role> roles,
        IEnumerable<PageRoleInfo> pageRoleInfos)
    {
        foreach (Role role in roles)
        {
            role.AppId = appId;
            role.Pages ??= [];
        }

        foreach (PageRoleInfo pageRoleInfo in pageRoleInfos)
        {
            Role role = roles.FirstOrDefault(predicate: candidate =>
                string.Equals(
                    a: candidate.Name,
                    b: pageRoleInfo.Role,
                    comparisonType: StringComparison.OrdinalIgnoreCase));

            if (role is null)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException(
                    message: $"Role '{pageRoleInfo.Role}' was not available for page-role import.");
            }

            role.Pages.Add(item: new PageRole
            {
                Page = new Page
                {
                    AppId = appId,
                    Path = pageRoleInfo.Path,
                },
            });
        }
    }
}