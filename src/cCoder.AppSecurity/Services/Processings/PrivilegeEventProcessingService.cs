// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class PrivilegeEventProcessingService(IPrivilegeEventService eventService) : IPrivilegeEventProcessingService
{
    public ValueTask RaisePrivilegeAddEventAsync(Privilege privilege) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaisePrivilegeAddEvent(
                privilege: privilege);

            return eventService.RaisePrivilegeAddEventAsync(privilege: privilege);
        });

    public ValueTask RaisePrivilegeUpdateEventAsync(Privilege privilege) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaisePrivilegeUpdateEvent(
                privilege: privilege);

            return eventService.RaisePrivilegeUpdateEventAsync(privilege: privilege);
        });

    public ValueTask RaisePrivilegeDeleteEventAsync(Privilege privilege) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaisePrivilegeDeleteEvent(
                privilege: privilege);

            return eventService.RaisePrivilegeDeleteEventAsync(privilege: privilege);
        });
}