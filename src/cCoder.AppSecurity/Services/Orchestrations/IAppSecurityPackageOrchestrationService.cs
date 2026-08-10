// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal interface IAppSecurityPackageOrchestrationService
{
    AppSecurityPackageMapping MapAppSecurityPackageMapping(AppSecurityPackageMapping mapping);
}