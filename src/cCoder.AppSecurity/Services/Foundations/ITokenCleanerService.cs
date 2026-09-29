// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace cCoder.AppSecurity.Services.Foundations;

internal interface ITokenCleanerService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}