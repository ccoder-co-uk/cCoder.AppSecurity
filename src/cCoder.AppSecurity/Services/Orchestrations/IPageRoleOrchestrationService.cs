// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal interface IPageRoleOrchestrationService
{
    ValueTask AddOrUpdateAppPageRolesAsync(App app);
}