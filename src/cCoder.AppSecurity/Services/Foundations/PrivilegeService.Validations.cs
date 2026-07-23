// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class PrivilegeService
{
    private static void ValidatePrivilegeOnGet(string privilegeId) =>
        ValidationRulesEngine.Validate(inputs: [
            privilegeId,
        ]);

    private static void ValidateAllOnGet(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidatePrivilegeOnAdd(Privilege newPrivilege) =>
        ValidationRulesEngine.Validate(inputs: [
            newPrivilege,
        ]);

    private static void ValidatePrivilegeOnUpdate(Privilege updatedPrivilege) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedPrivilege,
        ]);

    private static void ValidatePrivilegeOnDelete(string privilegeId) =>
        ValidationRulesEngine.Validate(inputs: [
            privilegeId,
        ]);
}