// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class PrivilegeEventProcessingService
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