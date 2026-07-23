// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class AccountRoleAssignmentProcessingService
{
    private static void ValidateAttachUsersRole(
        User user,
        int appId) =>
        ValidationRulesEngine.Validate(
            inputs:
            [
                user,
                appId,
            ]);
}