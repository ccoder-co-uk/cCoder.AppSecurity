// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class RoleService
{
    private static void ValidateRoleOnGet(Guid roleId) =>
        ValidationRulesEngine.Validate(inputs: [
            roleId,
        ]);

    private static void ValidateAllOnGet(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidateRoleOnAdd(Role newRole) =>
        ValidationRulesEngine.Validate(inputs: [
            newRole,
        ]);

    private static void ValidateValidatedRoleOnAdd(Role newRole) =>
        ValidationRulesEngine.Validate(inputs: [
            newRole,
        ]);

    private static void ValidateRoleOnUpdate(Role updatedRole) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedRole,
        ]);

    private static void ValidateValidatedRoleOnUpdate(Role updatedRole) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedRole,
        ]);

    private static void ValidateRoleOnDelete(Guid roleId) =>
        ValidationRulesEngine.Validate(inputs: [
            roleId,
        ]);

    private static void ValidateValidatedOnDelete(Guid roleId) =>
        ValidationRulesEngine.Validate(inputs: [
            roleId,
        ]);
}