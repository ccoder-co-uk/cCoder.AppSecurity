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
    public ValueTask RaiseRoleAddEventAsync(Role role) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseRoleAddEvent(
                role: role);

            return eventService.RaiseRoleAddEventAsync(role: role);
        });

    public ValueTask RaiseRoleUpdateEventAsync(Role role) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseRoleUpdateEvent(
                role: role);

            return eventService.RaiseRoleUpdateEventAsync(role: role);
        });

    public ValueTask RaiseRoleDeleteEventAsync(Role role) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseRoleDeleteEvent(
                role: role);

            return eventService.RaiseRoleDeleteEventAsync(role: role);
        });
}