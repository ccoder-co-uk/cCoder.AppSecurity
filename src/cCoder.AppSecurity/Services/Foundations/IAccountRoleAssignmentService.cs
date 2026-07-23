// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;

namespace cCoder.AppSecurity.Services.Foundations;

internal interface IAccountRoleAssignmentService
{
    AccountRoleAssignment GetAccountRoleAssignment(
        AccountRoleAssignment accountRoleAssignment);

    ValueTask<AccountRoleAssignment> AddAccountRoleAssignmentAsync(
        AccountRoleAssignment newAccountRoleAssignment);
}