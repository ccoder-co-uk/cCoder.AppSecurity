// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class UserService
{
    private static void ValidateGet(string userId) =>
        ValidationRulesEngine.Validate(inputs: [
            userId,
        ]);

    private static void ValidateGetByEmail(string email, bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            email,
            ignoreFilters,
        ]);

    private static void ValidateGetAll(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidateAddUser(User newUser) =>
        ValidationRulesEngine.Validate(inputs: [
            newUser,
        ]);

    private static void ValidateUpdateUser(User updatedUser) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedUser,
        ]);

    private static void ValidateDelete(string userId) =>
        ValidationRulesEngine.Validate(inputs: [
            userId,
        ]);
}