// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Orchestrations;

namespace cCoder.AppSecurity.Services.Aggregations;

internal sealed partial class AppSecurityMigrationAggregationService(
    IRoleOrchestrationService roleOrchestrationService,
    IJsonBroker jsonBroker
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
                    ? [jsonBroker.ParseJson<Role>(json: item.Data)]
                    : jsonBroker.ParseJson<Role[]>(json: item.Data);

                await roleOrchestrationService.ImportRoleAsync(appId: appId, roles: items);
            }

        });

    public AppSecurityPackage ExportPackage(int appId, string packageName) =>
        TryCatch(operation: AppSecurityPackage () =>
        {
            ValidateExportPackage(
                appId: appId,
                packageName: packageName);

            return packageName == "Roles"
            ? new AppSecurityPackage(name: "Roles")
            {
                Items =
                [
                    new AppSecurityPackageItem
                    {
                        Type = "Core/Role",
                        Data = roleOrchestrationService
                            .GetAll(ignoreFilters: false)
                            .Where(predicate: role => role.AppId == appId)
                            .Select(selector: role => new { role.Name, role.Privs })
                            .ToArray()
                            .ToJson(),
                    },
                ],
            }
            : new AppSecurityPackage(name: packageName) { Items = [] };
        });
}