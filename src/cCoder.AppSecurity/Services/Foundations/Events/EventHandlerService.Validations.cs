// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class EventHandlerService
{
    private static void ValidateListenToPackageEvents() =>
        ValidationRulesEngine.Validate(inputs: []);
}
