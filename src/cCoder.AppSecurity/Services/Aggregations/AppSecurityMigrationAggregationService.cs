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

            if (package.Items is null || !package.Items.Any(predicate: item =>
                item.Type is "Core/Role" or "AppSecurity/Role" or "ContentManagement/PageRole"))
            {
                return;
            }

            if (package.Items.Any(predicate: item =>
                item.Type is "Core/Role" or "AppSecurity/Role"))
            {
                await ImportRolesAsync(appId: appId, package: package);
            }

            if (!package.Items.Any(predicate: item =>
                item.Type == "ContentManagement/PageRole"))
            {
                return;
            }

            AppSecurityPackageMapping mapping = packageOrchestrationService
                .MapAppSecurityPackageMappingPageRoles(mapping: new AppSecurityPackageMapping
                {
                    AppId = appId,
                    Package = package,
                });

            App app = mapping.App;

            await pageRoleOrchestrationService.AddOrUpdateAppPageRolesAsync(app: app);
        });

    private async ValueTask ImportRolesAsync(
        int appId,
        AppSecurityPackage package)
    {
        AppSecurityPackageMapping mapping = packageOrchestrationService
            .MapAppSecurityPackageMappingRoles(mapping: new AppSecurityPackageMapping
            {
                AppId = appId,
                Package = package,
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