// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class UserRoleService
{
    private static void ValidateGetAll(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidateAddUserRole(UserRole newUserRole, bool authorize = true) =>
        ValidationRulesEngine.Validate(inputs: [
            newUserRole,
            authorize,
        ]);

    private static void ValidateDeleteUserRole(UserRole deletedUserRole) =>
        ValidationRulesEngine.Validate(inputs: [
            deletedUserRole,
        ]);
}