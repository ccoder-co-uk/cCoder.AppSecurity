// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AccountEventOrchestrationService
{
    private static void ValidateProcessSecurityAccountEvent(SecurityAccountEvent securityAccountEvent) =>
        ValidationRulesEngine.Validate(inputs: [
            securityAccountEvent,
        ]);
}