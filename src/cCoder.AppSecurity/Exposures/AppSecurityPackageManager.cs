// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Aggregations;


namespace cCoder.AppSecurity.Exposures;

internal class AppSecurityPackageManager(
    IAppSecurityMigrationAggregationService appSecurityMigrationAggregationService
) : IAppSecurityPackageManager, ICompositionExposure
{
    public ValueTask ImportPackageAsync(int appId, AppSecurityPackage appSecurityPackage) =>
        appSecurityMigrationAggregationService.ImportPackageAppSecurityPackageAsync(appId: appId, appSecurityPackage: appSecurityPackage);

    public AppSecurityPackage ExportPackage(int appId, string packageName) =>
        appSecurityMigrationAggregationService.ExportPackage(appId: appId, packageName: packageName);
}