// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class TokenCleanerOrchestrationService(
    ISecurityDbContextFactory ssoDbFactory,
    ILogger<TokenCleanerOrchestrationService> log)
    : ITokenCleanerOrchestrationService
{
    public Task RunAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async Task () =>
        {
            ValidateRun(
                cancellationToken: cancellationToken);

            using var sso = ssoDbFactory.CreateDbContext();

            Token[] expiredTokens = sso.Tokens
                .IgnoreQueryFilters()
                .Where(predicate: token => token.Expires < DateTimeOffset.UtcNow)
                .ToArray();

            if (expiredTokens.Length == 0)
            {
                return;
            }

            sso.RemoveRange(entities: expiredTokens);
            await sso.SaveChangesAsync(cancellationToken: cancellationToken);
            log.LogDebug(message: "{Count} Expired tokens removed", args: expiredTokens.Length);

        });
}