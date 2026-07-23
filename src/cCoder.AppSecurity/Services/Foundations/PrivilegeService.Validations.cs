// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class PrivilegeService
{
    private static void ValidateGet(string privilegeId) =>
        ValidationRulesEngine.Validate(inputs: [
            privilegeId,
        ]);

    private static void ValidateGetAll(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidateAddPrivilege(Privilege newPrivilege) =>
        ValidationRulesEngine.Validate(inputs: [
            newPrivilege,
        ]);

    private static void ValidateUpdatePrivilege(Privilege updatedPrivilege) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedPrivilege,
        ]);

    private static void ValidateDelete(string privilegeId) =>
        ValidationRulesEngine.Validate(inputs: [
            privilegeId,
        ]);
}