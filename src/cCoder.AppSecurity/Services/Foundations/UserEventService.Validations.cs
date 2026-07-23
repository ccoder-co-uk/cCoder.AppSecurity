// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class UserEventService
{
    private static void ValidateRaiseUserAddEvent(User entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);

    private static void ValidateRaiseUserUpdateEvent(User entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);

    private static void ValidateRaiseUserDeleteEvent(User entity) =>
        ValidationRulesEngine.Validate(inputs: [
            entity,
        ]);
}