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
    public ValueTask AddAsync(App newApp) =>
        appOrchestrationService.AddAppAsync(app: newApp);
    public ValueTask UpdateAsync(App updatedApp) =>
        appOrchestrationService.UpdateAppAsync(app: updatedApp);
    public ValueTask DeleteAsync(int appId) =>
        appOrchestrationService.DeleteAsync(appId: appId);
}