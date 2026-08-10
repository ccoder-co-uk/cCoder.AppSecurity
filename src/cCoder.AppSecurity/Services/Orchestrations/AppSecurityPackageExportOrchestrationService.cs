// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Processings;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AppSecurityPackageExportOrchestrationService(
    IRoleProcessingService roleProcessingService,
    IJsonProcessingService jsonProcessingService)
        : IAppSecurityPackageExportOrchestrationService
{
    public AppSecurityPackage ExportAppSecurityPackage(int appId, string packageName) =>
        TryCatch(operation: AppSecurityPackage () =>
        {
            ValidateAppSecurityPackageOnExport(appId: appId, packageName: packageName);

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
                                value: roleProcessingService.GetAll(ignoreFilters: true)
                                    .Where(predicate: role => role.AppId == appId)
                                    .Select(selector: role => new { role.Name, role.Privs })
                                    .ToArray()),
                        },
                    ],
                }
                : new AppSecurityPackage { Name = packageName, Items = [] };
        });
}