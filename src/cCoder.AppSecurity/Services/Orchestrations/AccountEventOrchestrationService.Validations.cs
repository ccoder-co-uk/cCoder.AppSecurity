// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AccountEventOrchestrationService
{
    private static void ValidateProcessSecurityAccountEvent(SecurityAccountEvent accountEvent) =>
        ValidationRulesEngine.Validate(inputs: [
            accountEvent,
        ]);
}