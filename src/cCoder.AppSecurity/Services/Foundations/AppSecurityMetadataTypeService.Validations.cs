// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AppSecurityMetadataTypeService
{
    private static void ValidateGetKnownMetadata() =>
        ValidationRulesEngine.Validate(inputs: []);
}