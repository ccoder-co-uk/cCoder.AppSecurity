// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Services.Processings;

internal interface IAnalysePlatformUsageProcessingService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}