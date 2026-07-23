// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AppOrchestrationService
{
    private static void ValidateAddApp(App newApp) =>
        ValidationRulesEngine.Validate(inputs: [
            newApp,
        ]);

    private static void ValidateUpdateApp(App updatedApp) =>
        ValidationRulesEngine.Validate(inputs: [
            updatedApp,
        ]);

    private static void ValidateDelete(int appId) =>
        ValidationRulesEngine.Validate(inputs: [
            appId,
        ]);
}