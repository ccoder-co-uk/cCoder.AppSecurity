// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class UserEventProcessingService
{
    private static void ValidateRaiseUserAddEvent(User user) =>
        ValidationRulesEngine.Validate(inputs: [
            user,
        ]);

    private static void ValidateRaiseUserUpdateEvent(User user) =>
        ValidationRulesEngine.Validate(inputs: [
            user,
        ]);

    private static void ValidateRaiseUserDeleteEvent(User user) =>
        ValidationRulesEngine.Validate(inputs: [
            user,
        ]);
}