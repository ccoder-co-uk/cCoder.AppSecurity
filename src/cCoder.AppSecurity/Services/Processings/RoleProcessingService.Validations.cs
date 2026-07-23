// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class RoleProcessingService
{
    private static void ValidateGet(Guid roleId) =>
        ValidationRulesEngine.Validate(inputs: [
            roleId,
        ]);

    private static void ValidateGetAll(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidateAddRole(Role newRole) =>
        ValidationRulesEngine.Validate(inputs: [
            newRole,
        ]);

    private static void ValidateAddValidatedRole(Role newRole) =>
        ValidationRulesEngine.Validate(inputs: [
            newRole,
        ]);

    private static void ValidateUpdateRole(Role updatedRole) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedRole,
        ]);

    private static void ValidateUpdateValidatedRole(Role updatedRole) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedRole,
        ]);

    private static void ValidateDelete(Guid roleId) =>
        ValidationRulesEngine.Validate(inputs: [
            roleId,
        ]);

    private static void ValidateDeleteValidated(Guid roleId) =>
        ValidationRulesEngine.Validate(inputs: [
            roleId,
        ]);

    private static void ValidateDeleteAllRole(IEnumerable<Role> deletedRole) =>
        ValidationRulesEngine.Validate(inputs: [
            deletedRole,
        ]);
}