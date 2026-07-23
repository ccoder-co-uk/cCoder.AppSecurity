// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class AnalysePlatformUsageProcessingService
{
    private static void ValidateRun(CancellationToken cancellationToken = default) =>
        ValidationRulesEngine.Validate(inputs: [
            cancellationToken,
        ]);
}