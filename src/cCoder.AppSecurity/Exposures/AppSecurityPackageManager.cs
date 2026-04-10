using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Aggregations;


namespace cCoder.AppSecurity.Exposures;

internal class AppSecurityPackageManager(
    IAppSecurityMigrationAggregationService appSecurityMigrationAggregationService
) : IAppSecurityPackageManager
{
    public ValueTask ImportPackageAsync(int appId, AppSecurityPackage package) =>
        appSecurityMigrationAggregationService.ImportPackageAsync(appId, package);

    public AppSecurityPackage ExportPackage(int appId, string packageName) =>
        appSecurityMigrationAggregationService.ExportPackage(appId, packageName);
}


