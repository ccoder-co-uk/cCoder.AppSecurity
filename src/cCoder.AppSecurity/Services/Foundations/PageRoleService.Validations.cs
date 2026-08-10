// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class PageRoleService
{
    private static void ValidateAllOnGet() =>
        ValidationRulesEngine.Validate(inputs: []);

    private static void ValidatePageIdOnGet(int appId, string path) =>
        ValidationRulesEngine.Validate(inputs: [appId]);

    private static void ValidatePageRoleOnAdd(PageRole newPageRole) =>
        ValidationRulesEngine.Validate(inputs: [newPageRole]);
}