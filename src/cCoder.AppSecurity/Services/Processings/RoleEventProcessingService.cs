// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class RoleEventProcessingService(IRoleEventService eventService) : IRoleEventProcessingService
{
    public ValueTask RaiseRoleAddEventAsync(Role entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseRoleAddEvent(
                entity: entity);

            return eventService.RaiseRoleAddEventAsync(entity: entity);
        });

    public ValueTask RaiseRoleUpdateEventAsync(Role entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseRoleUpdateEvent(
                entity: entity);

            return eventService.RaiseRoleUpdateEventAsync(entity: entity);
        });

    public ValueTask RaiseRoleDeleteEventAsync(Role entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseRoleDeleteEvent(
                entity: entity);

            return eventService.RaiseRoleDeleteEventAsync(entity: entity);
        });
}