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
        AppSecurityPackage appSecurityPackage) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateImportPackageAppSecurityPackage(appId: appId, appSecurityPackage: appSecurityPackage);

            if (appSecurityPackage.Items is null || !appSecurityPackage.Items.Any(predicate: item =>
                item.Type is "Core/Role" or "AppSecurity/Role" or "ContentManagement/PageRole"))
            {
                return;
            }

            if (appSecurityPackage.Items.Any(predicate: item =>
                item.Type is "Core/Role" or "AppSecurity/Role"))
            {
                await ImportRolesAsync(appId: appId, appSecurityPackage: appSecurityPackage);
            }

            if (!appSecurityPackage.Items.Any(predicate: item =>
                item.Type == "ContentManagement/PageRole"))
            {
                return;
            }

            AppSecurityPackageMapping mapping = packageOrchestrationService
                .MapAppSecurityPackageMappingPageRoles(appSecurityPackageMapping: new AppSecurityPackageMapping
                {
                    AppId = appId,
                    Package = appSecurityPackage,
                });

            App app = mapping.App;

            await pageRoleOrchestrationService.AddOrUpdateAppPageRolesAsync(app: app);
        });

    private async ValueTask ImportRolesAsync(
        int appId,
        AppSecurityPackage appSecurityPackage)
    {
        AppSecurityPackageMapping mapping = packageOrchestrationService
            .MapAppSecurityPackageMappingRoles(appSecurityPackageMapping: new AppSecurityPackageMapping
            {
                AppId = appId,
                Package = appSecurityPackage,
            });

        await appOrchestrationService.UpdateAppAsync(app: mapping.App);
    }

    public AppSecurityPackage ExportPackage(int appId, string packageName) =>
        TryCatch(operation: AppSecurityPackage () =>
        {
            ValidateExportPackage(appId: appId, packageName: packageName);

            return packageExportOrchestrationService.ExportAppSecurityPackage(
                appId: appId,
                packageName: packageName);
        });
}