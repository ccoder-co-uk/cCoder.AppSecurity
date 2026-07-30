// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Services.Foundations;

internal interface ITokenCleanerService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}