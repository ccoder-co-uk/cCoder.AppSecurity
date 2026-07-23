// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Orchestrations;

public interface IAppOrchestrationService
{
    ValueTask AddAppAsync(App app);
    ValueTask UpdateAppAsync(App app);
    ValueTask DeleteAsync(int appId);
}