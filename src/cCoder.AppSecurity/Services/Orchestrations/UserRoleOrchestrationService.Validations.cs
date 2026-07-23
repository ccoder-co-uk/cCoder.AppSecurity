// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class UserRoleOrchestrationService
{
    private static void ValidateGetAll(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidateAddUserRole(UserRole newUserRole) =>
        ValidationRulesEngine.Validate(inputs: [
            newUserRole,
        ]);

    private static void ValidateDeleteUserRole(UserRole deletedUserRole) =>
        ValidationRulesEngine.Validate(inputs: [
            deletedUserRole,
        ]);

    private static void ValidateAddOrUpdateUserRole(IEnumerable<UserRole> items) =>
        ValidationRulesEngine.Validate(inputs: [
            items,
        ]);

    private static void ValidateDeleteAllUserRole(IEnumerable<UserRole> deletedUserRole) =>
        ValidationRulesEngine.Validate(inputs: [
            deletedUserRole,
        ]);

    private static void ValidateSaveUserRole(UserRole entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);
}