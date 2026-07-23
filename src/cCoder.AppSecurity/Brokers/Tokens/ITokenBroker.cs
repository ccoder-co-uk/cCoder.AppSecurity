// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Brokers.Tokens;

internal interface ITokenBroker
{
    Task DeleteExpiredTokensAsync(CancellationToken cancellationToken);
}