// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class AppProcessingService
{
    private static void ValidateGetAll() =>
        ValidationRulesEngine.Validate(inputs: []);

    private static void ValidateGetByDomain(string domain) =>
        ValidationRulesEngine.Validate(inputs: [
            domain,
        ]);
}