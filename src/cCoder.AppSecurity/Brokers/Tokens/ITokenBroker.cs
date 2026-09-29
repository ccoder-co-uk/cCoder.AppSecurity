// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace cCoder.AppSecurity.Brokers.Tokens;

internal interface ITokenBroker
{
    Task DeleteExpiredTokensAsync(CancellationToken cancellationToken);
}