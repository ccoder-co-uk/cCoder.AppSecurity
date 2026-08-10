// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class PageRoleOrchestrationService
{
    private static void ValidatePageRolesOnAddOrUpdate(App app) =>
        ValidationRulesEngine.Validate(inputs: [app]);
}