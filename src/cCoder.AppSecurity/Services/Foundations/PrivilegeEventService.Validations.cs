// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class PrivilegeEventService
{
    private static void ValidateRaisePrivilegeAddEvent(Privilege entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);

    private static void ValidateRaisePrivilegeUpdateEvent(Privilege entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);

    private static void ValidateRaisePrivilegeDeleteEvent(Privilege entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);
}