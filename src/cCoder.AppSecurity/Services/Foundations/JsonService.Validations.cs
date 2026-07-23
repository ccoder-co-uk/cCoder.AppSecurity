// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class JsonService
{
    private static void ValidateJsonOnParse(string json) =>
        ValidationRulesEngine.Validate(
            inputs:
            [
                json,
            ]);

    private static void ValidateValueOnSerialize(object value) =>
        ValidationRulesEngine.Validate(
            inputs:
            [
                value,
            ]);
}