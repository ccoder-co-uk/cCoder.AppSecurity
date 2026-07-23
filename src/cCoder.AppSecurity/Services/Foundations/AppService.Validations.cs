// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AppService
{
    private static void ValidateGetAll() =>
        ValidationRulesEngine.Validate(inputs: []);

    private static void ValidateGetByDomain(string domain) =>
        ValidationRulesEngine.Validate(inputs: [
            domain,
        ]);
}