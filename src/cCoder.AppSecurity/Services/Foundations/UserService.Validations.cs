// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class UserService
{
    private static void ValidateUserOnGet(string userId) =>
        ValidationRulesEngine.Validate(inputs: [
            userId,
        ]);

    private static void ValidateByEmailOnGet(string email, bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            email,
            ignoreFilters,
        ]);

    private static void ValidateAllOnGet(bool ignoreFilters = false) =>
        ValidationRulesEngine.Validate(inputs: [
            ignoreFilters,
        ]);

    private static void ValidateUserOnAdd(User newUser) =>
        ValidationRulesEngine.Validate(inputs: [
            newUser,
        ]);

    private static void ValidateUserOnUpdate(User updatedUser) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedUser,
        ]);

    private static void ValidateUserOnDelete(string userId) =>
        ValidationRulesEngine.Validate(inputs: [
            userId,
        ]);
}