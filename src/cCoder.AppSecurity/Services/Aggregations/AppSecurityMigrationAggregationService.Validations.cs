// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Aggregations;

internal sealed partial class AppSecurityMigrationAggregationService
{
    private static void ValidateImportPackageAppSecurityPackage(int appId, AppSecurityPackage appSecurityPackage) =>
        ValidationRulesEngine.Validate(inputs: [
            appId,
            appSecurityPackage,
        ]);

    private static void ValidateExportPackage(int appId, string packageName) =>
        ValidationRulesEngine.Validate(inputs: [
            appId,
            packageName,
        ]);
}