// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Events;

namespace cCoder.AppSecurity.Services.Orchestrations;

public interface IAccountEventOrchestrationService
{
    ValueTask ProcessSecurityAccountEventAsync(SecurityAccountEvent accountEvent);
}