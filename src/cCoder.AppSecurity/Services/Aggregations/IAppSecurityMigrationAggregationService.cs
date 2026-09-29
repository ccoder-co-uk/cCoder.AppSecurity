// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.AppSecurity.Models;


namespace cCoder.AppSecurity.Services.Aggregations;

internal interface IAppSecurityMigrationAggregationService
{
    ValueTask ImportPackageAppSecurityPackageAsync(int appId, AppSecurityPackage appSecurityPackage);
    AppSecurityPackage ExportPackage(int appId, string packageName);
}