// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;

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