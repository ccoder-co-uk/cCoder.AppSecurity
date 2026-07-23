// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class EventHandlerService
{
    private static void ValidateListenToAllEvents() =>
        ValidationRulesEngine.Validate(inputs: []);

    private static void ValidateListenToAppCreateAndUpdateEvents() =>
        ValidationRulesEngine.Validate(inputs: []);

    private static void ValidateListenToAppDeleteEvents() =>
        ValidationRulesEngine.Validate(inputs: []);

    private static void ValidateListenToSecurityAccountEvents() =>
        ValidationRulesEngine.Validate(inputs: []);
}