// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Security.Objects.Events;

namespace cCoder.AppSecurity.Services.Processings.Events;

internal class AccountEventProcessingService(IAccountEventOrchestrationService orchestrationService)
    : IAccountEventProcessingService
{
    public ValueTask ProcessAsync(SecurityAccountEvent accountEvent) =>
        orchestrationService.ProcessAsync(accountEvent);
}