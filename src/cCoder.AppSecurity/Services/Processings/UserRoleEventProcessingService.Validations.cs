// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class UserRoleEventProcessingService
{
    private static void ValidateRaiseUserRoleAddEvent(UserRole entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);

    private static void ValidateRaiseUserRoleDeleteEvent(UserRole entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);
}