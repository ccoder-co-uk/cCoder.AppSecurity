// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;
using cCoder.AppSecurity.Models;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AppSecurityPackageOrchestrationService
{
    private static void ValidateAppSecurityPackageMappingOnMap(AppSecurityPackageMapping mapping) =>
        ValidationRulesEngine.Validate(inputs: [mapping]);
}