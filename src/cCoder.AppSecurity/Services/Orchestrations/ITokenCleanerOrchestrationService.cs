// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Services.Orchestrations;

public interface ITokenCleanerOrchestrationService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}