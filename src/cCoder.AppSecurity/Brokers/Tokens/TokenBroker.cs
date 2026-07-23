// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace cCoder.AppSecurity.Brokers.Tokens;

internal sealed class TokenBroker(
    ISecurityDbContextFactory securityDbContextFactory)
    : ITokenBroker
{
    public async Task DeleteExpiredTokensAsync(
        CancellationToken cancellationToken)
    {
        await using var securityDbContext =
            securityDbContextFactory.CreateDbContext();

        await securityDbContext.Tokens
            .IgnoreQueryFilters()
            .Where(predicate: token => token.Expires < DateTimeOffset.UtcNow)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }
}