// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AppSecurityPackageExportOrchestrationService
{
    private static void ValidateAppSecurityPackageOnExport(int appId, string packageName) =>
        ValidationRulesEngine.Validate(inputs: [appId, packageName]);
}