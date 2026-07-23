// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Services.Foundations;

public interface ITokenCleanerService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
