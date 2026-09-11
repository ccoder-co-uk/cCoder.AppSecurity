// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Aggregations;


namespace cCoder.AppSecurity.Exposures;

internal class AppSecurityPackageManager(
    IAppSecurityMigrationAggregationService appSecurityMigrationAggregationService
) : IAppSecurityPackageManager
{
    public ValueTask ImportPackageAsync(int appId, AppSecurityPackage appSecurityPackage) =>
        appSecurityMigrationAggregationService.ImportPackageAppSecurityPackageAsync(appId: appId, appSecurityPackage: appSecurityPackage);

    public AppSecurityPackage ExportPackage(int appId, string packageName) =>
        appSecurityMigrationAggregationService.ExportPackage(appId: appId, packageName: packageName);
}