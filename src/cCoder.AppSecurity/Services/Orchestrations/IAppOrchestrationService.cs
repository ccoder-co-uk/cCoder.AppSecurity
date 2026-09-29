// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal interface IAppOrchestrationService
{
    ValueTask AddAppAsync(App app);
    ValueTask UpdateAppAsync(App app);
    ValueTask DeleteAsync(int appId);
}