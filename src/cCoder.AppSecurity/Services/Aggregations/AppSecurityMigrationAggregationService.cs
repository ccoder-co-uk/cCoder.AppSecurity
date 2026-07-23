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

internal class AppSecurityMigrationAggregationService(
    IRoleOrchestrationService roleOrchestrationService,
    IJsonBroker jsonBroker
) : IAppSecurityMigrationAggregationService
{
    public async ValueTask ImportPackageAsync(int appId, AppSecurityPackage package)
    {
        if (package.Items is null || package.Items.Count == 0)
            return;

        foreach (AppSecurityPackageItem item in package.Items.Where(item => item.Type == "Core/Role"))
        {
            Role[] items = item.Data.StartsWith("{")
                ? [jsonBroker.ParseJson<Role>(item.Data)]
                : jsonBroker.ParseJson<Role[]>(item.Data);

            await roleOrchestrationService.ImportAsync(appId, items);
        }
    }

    public AppSecurityPackage ExportPackage(int appId, string packageName) =>
        packageName == "Roles"
            ? new AppSecurityPackage("Roles")
            {
                Items =
                [
                    new AppSecurityPackageItem
                    {
                        Type = "Core/Role",
                        Data = roleOrchestrationService
                            .GetAll(false)
                            .Where(role => role.AppId == appId)
                            .Select(role => new { role.Name, role.Privs })
                            .ToArray()
                            .ToJson(),
                    },
                ],
            }
            : new AppSecurityPackage(packageName) { Items = [] };
}