// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
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