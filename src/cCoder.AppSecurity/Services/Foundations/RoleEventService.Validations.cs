// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class RoleEventService
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