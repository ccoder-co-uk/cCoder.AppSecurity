// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AppService
{

    private static void ValidateByDomainOnGet(string domain) =>
        ValidationRulesEngine.Validate(inputs: [
            domain,
        ]);
}