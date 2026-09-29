// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Security.Models.Events;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal interface IAccountEventOrchestrationService
{
    ValueTask ProcessSecurityAccountEventAsync(SecurityAccountEvent securityAccountEvent);
}