// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class AccountRoleAssignmentProcessingService(
    IAccountRoleAssignmentService accountRoleAssignmentService)
    : IAccountRoleAssignmentProcessingService
{
    public ValueTask AttachUsersRoleAsync(
        User user,
        int appId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateAttachUsersRole(
                user: user,
                appId: appId);

            AccountRoleAssignment assignment =
                accountRoleAssignmentService.GetAccountRoleAssignment(
                    accountRoleAssignment: new AccountRoleAssignment
                    {
                        AppId = appId,
                        UserId = user.Id,
                    });

            if (assignment.RoleId.HasValue
                && !assignment.IsAssigned)
            {
                await accountRoleAssignmentService
                    .AddAccountRoleAssignmentAsync(
                        newAccountRoleAssignment: assignment);
            }
        });
}