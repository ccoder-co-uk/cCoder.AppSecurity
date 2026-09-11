// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class UserEventProcessingService(IUserEventService eventService) : IUserEventProcessingService
{
    public ValueTask RaiseUserAddEventAsync(User user) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserAddEvent(
                user: user);

            return eventService.RaiseUserAddEventAsync(user: user);
        });

    public ValueTask RaiseUserUpdateEventAsync(User user) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserUpdateEvent(
                user: user);

            return eventService.RaiseUserUpdateEventAsync(user: user);
        });

    public ValueTask RaiseUserDeleteEventAsync(User user) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserDeleteEvent(
                user: user);

            return eventService.RaiseUserDeleteEventAsync(user: user);
        });
}