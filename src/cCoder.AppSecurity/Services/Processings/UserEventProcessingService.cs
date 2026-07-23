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
    public ValueTask RaiseUserAddEventAsync(User entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserAddEvent(
                entity: entity);

            return eventService.RaiseUserAddEventAsync(entity: entity);
        });

    public ValueTask RaiseUserUpdateEventAsync(User entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserUpdateEvent(
                entity: entity);

            return eventService.RaiseUserUpdateEventAsync(entity: entity);
        });

    public ValueTask RaiseUserDeleteEventAsync(User entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserDeleteEvent(
                entity: entity);

            return eventService.RaiseUserDeleteEventAsync(entity: entity);
        });
}