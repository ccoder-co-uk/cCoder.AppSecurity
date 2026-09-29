// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace cCoder.AppSecurity.Services.Processings;

internal interface IAnalysePlatformUsageProcessingService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}