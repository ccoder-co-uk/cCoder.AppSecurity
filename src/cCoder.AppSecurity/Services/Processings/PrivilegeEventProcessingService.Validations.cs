// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class PrivilegeEventProcessingService
{
    private static void ValidateRaisePrivilegeAddEvent(Privilege privilege) =>
        ValidationRulesEngine.Validate(inputs: [
            privilege,
        ]);

    private static void ValidateRaisePrivilegeUpdateEvent(Privilege privilege) =>
        ValidationRulesEngine.Validate(inputs: [
            privilege,
        ]);

    private static void ValidateRaisePrivilegeDeleteEvent(Privilege privilege) =>
        ValidationRulesEngine.Validate(inputs: [
            privilege,
        ]);
}