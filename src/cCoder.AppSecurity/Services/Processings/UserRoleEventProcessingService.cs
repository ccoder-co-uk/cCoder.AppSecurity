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
    public ValueTask RaiseUserRoleAddEventAsync(UserRole userRole) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserRoleAddEvent(
                userRole: userRole);

            return eventService.RaiseUserRoleAddEventAsync(userRole: userRole);
        });

    public ValueTask RaiseUserRoleDeleteEventAsync(UserRole userRole) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaiseUserRoleDeleteEvent(
                userRole: userRole);

            return eventService.RaiseUserRoleDeleteEventAsync(userRole: userRole);
        });
}