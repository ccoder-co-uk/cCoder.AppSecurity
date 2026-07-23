// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Security.Objects.Events;

namespace cCoder.AppSecurity.Services.Processings.Events;

internal sealed partial class AccountEventProcessingService(IAccountEventOrchestrationService orchestrationService)
    : IAccountEventProcessingService
{
    public ValueTask ProcessSecurityAccountEventAsync(SecurityAccountEvent accountEvent) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateProcessSecurityAccountEvent(
                accountEvent: accountEvent);

            return orchestrationService.ProcessSecurityAccountEventAsync(accountEvent: accountEvent);
        });
}