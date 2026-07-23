// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AnalysePlatformUsageOrchestrationService
{
    private static void ValidateRun(CancellationToken cancellationToken = default) =>
        ValidationRulesEngine.Validate(inputs: [
            cancellationToken,
        ]);
}