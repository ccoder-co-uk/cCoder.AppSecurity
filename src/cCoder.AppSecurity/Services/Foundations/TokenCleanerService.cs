// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Tokens;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class TokenCleanerService(
    ITokenBroker tokenBroker)
    : ITokenCleanerService
{
    public Task RunAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
        {
            ValidateRun(
                cancellationToken: cancellationToken);

            await tokenBroker.DeleteExpiredTokensAsync(
                cancellationToken: cancellationToken);
        });
}