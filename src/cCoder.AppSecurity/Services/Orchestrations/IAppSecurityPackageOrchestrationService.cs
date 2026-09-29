// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal interface IAppSecurityPackageOrchestrationService
{
    AppSecurityPackageMapping MapAppSecurityPackageMappingRoles(AppSecurityPackageMapping appSecurityPackageMapping);
    AppSecurityPackageMapping MapAppSecurityPackageMappingPageRoles(AppSecurityPackageMapping appSecurityPackageMapping);
}