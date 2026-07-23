// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Services.Processings;

public interface IAnalysePlatformUsageProcessingService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
