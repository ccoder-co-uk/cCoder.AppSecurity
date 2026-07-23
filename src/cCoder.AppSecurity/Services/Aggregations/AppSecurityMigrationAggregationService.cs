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

            foreach (AppSecurityPackageItem item in package.Items.Where(predicate: item => item.Type == "Core/Role"))
            {
                Role[] items = item.Data.StartsWith(value: "{")
                    ? [jsonProcessingService.ParseJson<Role>(json: item.Data)]
                    : jsonProcessingService.ParseJson<Role[]>(json: item.Data);

                await ImportRolesAsync(
                    appId: appId,
                    roles: items);
            }

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
                            .GetAll(ignoreFilters: false)
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
            .GetAll(ignoreFilters: false)
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
}