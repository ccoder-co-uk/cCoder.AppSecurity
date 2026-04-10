using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed class TokenCleanerOrchestrationService(
    ISecurityDbContextFactory ssoDbFactory,
    ILogger<TokenCleanerOrchestrationService> log)
    : ITokenCleanerOrchestrationService
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        using var sso = ssoDbFactory.CreateDbContext();

        Token[] expiredTokens = sso.Tokens
            .IgnoreQueryFilters()
            .Where(token => token.Expires < DateTimeOffset.UtcNow)
            .ToArray();

        if (expiredTokens.Length == 0)
            return;

        sso.RemoveRange(expiredTokens);
        await sso.SaveChangesAsync(cancellationToken);
        log.LogDebug("{Count} Expired tokens removed", expiredTokens.Length);
    }
}
