// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Orchestrations;

namespace cCoder.AppSecurity.Exposures;

internal class AppSecurityAppExposure(IAppOrchestrationService appOrchestrationService)
    : IAppSecurityAppExposure
{
    public ValueTask AddAsync(App app) => appOrchestrationService.AddAsync(app: app);
    public ValueTask UpdateAsync(App app) => appOrchestrationService.UpdateAsync(app: app);
    public ValueTask DeleteAsync(int appId) => appOrchestrationService.DeleteAsync(appId: appId);
}