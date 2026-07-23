// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;
using cCoder.AppSecurity.Models;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AccountRoleAssignmentService
{
    private static void ValidateAccountRoleAssignmentOnGet(
        AccountRoleAssignment accountRoleAssignment) =>
        ValidationRulesEngine.Validate(
            inputs:
            [
                accountRoleAssignment,
            ]);

    private static void ValidateAccountRoleAssignmentOnAdd(
        AccountRoleAssignment newAccountRoleAssignment) =>
        ValidationRulesEngine.Validate(
            inputs:
            [
                newAccountRoleAssignment,
            ]);
}