// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class UserRoleService
{
    private static void ValidateAllOnGet(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidateUserRoleOnAdd(UserRole newUserRole, bool authorize = true) =>
        ValidationRulesEngine.Validate(inputs: [
            newUserRole,
            authorize,
        ]);

    private static void ValidateUserRoleOnDelete(UserRole deletedUserRole) =>
        ValidationRulesEngine.Validate(inputs: [
            deletedUserRole,
        ]);
}