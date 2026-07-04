using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;

namespace cCoder.AppSecurity.Services.Aggregations;

internal class AppSecurityMigrationAggregationService(
    IRoleProcessingService roleProcessingService,
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

            var dbVersions = roleProcessingService
                .GetAll(false)
                .Where(role => role.AppId == appId)
                .Select(role => new { role.Id, role.Name })
                .ToArray();

            foreach (Role role in items)
            {
                role.AppId = appId;
                role.Id = dbVersions.FirstOrDefault(existing => existing.Name == role.Name)?.Id ?? Guid.Empty;

                if (role.Id == Guid.Empty)
                    await roleProcessingService.AddValidatedAsync(role);
                else
                    await roleProcessingService.UpdateValidatedAsync(role);
            }
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
                        Data = roleProcessingService
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
