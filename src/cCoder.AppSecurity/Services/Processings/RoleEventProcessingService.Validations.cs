// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class RoleEventProcessingService
{
    private static void ValidateRaiseRoleAddEvent(Role entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);

    private static void ValidateRaiseRoleUpdateEvent(Role entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);

    private static void ValidateRaiseRoleDeleteEvent(Role entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);
}