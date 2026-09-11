// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;
using cCoder.AppSecurity.Models;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AppSecurityPackageOrchestrationService
{
    private static void ValidateAppSecurityPackageMappingRolesOnMap(AppSecurityPackageMapping appSecurityPackageMapping) =>
        ValidationRulesEngine.Validate(inputs: [appSecurityPackageMapping]);

    private static void ValidateAppSecurityPackageMappingPageRolesOnMap(AppSecurityPackageMapping appSecurityPackageMapping) =>
        ValidationRulesEngine.Validate(inputs: [appSecurityPackageMapping]);
}