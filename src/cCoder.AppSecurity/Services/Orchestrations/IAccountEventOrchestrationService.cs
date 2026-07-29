// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Events;

namespace cCoder.AppSecurity.Services.Orchestrations;

public interface IAccountEventOrchestrationService
{
    ValueTask ProcessSecurityAccountEventAsync(SecurityAccountEvent accountEvent);
}