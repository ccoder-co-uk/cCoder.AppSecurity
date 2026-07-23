// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings.Events;

internal sealed partial class AccountEventProcessingService
{
    private static void ValidateProcessSecurityAccountEvent(SecurityAccountEvent accountEvent) =>
        ValidationRulesEngine.Validate(inputs: [
            accountEvent,
        ]);
}