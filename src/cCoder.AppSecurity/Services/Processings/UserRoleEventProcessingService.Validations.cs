// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class UserRoleEventProcessingService
{
    private static void ValidateRaiseUserRoleAddEvent(UserRole userRole) =>
        ValidationRulesEngine.Validate(inputs: [
            userRole,
        ]);

    private static void ValidateRaiseUserRoleDeleteEvent(UserRole userRole) =>
        ValidationRulesEngine.Validate(inputs: [
            userRole,
        ]);
}