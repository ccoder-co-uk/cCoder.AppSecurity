// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AnalysePlatformUsageService
{
    private static void ValidateValueOnSerialize(object value) =>
        ValidationRulesEngine.Validate(inputs: [value]);
}