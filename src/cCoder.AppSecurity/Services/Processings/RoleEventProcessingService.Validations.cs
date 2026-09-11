// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class RoleEventProcessingService
{
    private static void ValidateRaiseRoleAddEvent(Role role) =>
        ValidationRulesEngine.Validate(inputs: [
            role,
        ]);

    private static void ValidateRaiseRoleUpdateEvent(Role role) =>
        ValidationRulesEngine.Validate(inputs: [
            role,
        ]);

    private static void ValidateRaiseRoleDeleteEvent(Role role) =>
        ValidationRulesEngine.Validate(inputs: [
            role,
        ]);
}