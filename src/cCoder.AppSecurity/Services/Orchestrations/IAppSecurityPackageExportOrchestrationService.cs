// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal interface IAppSecurityPackageExportOrchestrationService
{
    AppSecurityPackage ExportAppSecurityPackage(int appId, string packageName);
}