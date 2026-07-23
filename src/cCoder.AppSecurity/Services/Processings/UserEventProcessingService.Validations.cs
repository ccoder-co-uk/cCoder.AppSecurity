// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class UserEventProcessingService
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