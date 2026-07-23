// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class TokenCleanerService
{
    private static void ValidateRun(
        CancellationToken cancellationToken = default) =>
        ValidationRulesEngine.Validate(
            inputs:
            [
                cancellationToken,
            ]);
}