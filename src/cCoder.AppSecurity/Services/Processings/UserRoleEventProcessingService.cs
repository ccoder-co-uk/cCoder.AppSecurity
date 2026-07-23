// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class UserRoleEventProcessingService(IUserRoleEventService eventService) : IUserRoleEventProcessingService
{
    public ValueTask RaiseUserRoleAddEventAsync(UserRole entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserRoleAddEvent(
                entity: entity);

            return eventService.RaiseUserRoleAddEventAsync(entity: entity);
        });

    public ValueTask RaiseUserRoleDeleteEventAsync(UserRole entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserRoleDeleteEvent(
                entity: entity);

            return eventService.RaiseUserRoleDeleteEventAsync(entity: entity);
        });
}