// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Aggregations;

internal sealed partial class AppSecurityMigrationAggregationService(
    IAppSecurityPackageOrchestrationService packageOrchestrationService,
    IAppSecurityPackageExportOrchestrationService packageExportOrchestrationService,
    IAppOrchestrationService appOrchestrationService,
    IPageRoleOrchestrationService pageRoleOrchestrationService)
        : IAppSecurityMigrationAggregationService
{
    public ValueTask ImportPackageAppSecurityPackageAsync(
        int appId,
        AppSecurityPackage package) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateImportPackageAppSecurityPackage(appId: appId, package: package);

            if (package.Items is null || package.Items.Count == 0)
            {
                return;
            }

            AppSecurityPackageMapping mapping = packageOrchestrationService
                .MapAppSecurityPackageMapping(mapping: new AppSecurityPackageMapping
                {
                    AppId = appId,
                    Package = package,
                });

            App app = mapping.App;

            await appOrchestrationService.UpdateAppAsync(app: app);
            await pageRoleOrchestrationService.AddOrUpdateAppPageRolesAsync(app: app);
        });

    public AppSecurityPackage ExportPackage(int appId, string packageName) =>
        TryCatch(operation: AppSecurityPackage () =>
        {
            ValidateExportPackage(appId: appId, packageName: packageName);

            return packageExportOrchestrationService.ExportAppSecurityPackage(
                appId: appId,
                packageName: packageName);
        });
}