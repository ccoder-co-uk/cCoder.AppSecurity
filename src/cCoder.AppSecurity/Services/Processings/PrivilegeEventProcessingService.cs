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
    public ValueTask RaisePrivilegeAddEventAsync(Privilege entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaisePrivilegeAddEvent(
                entity: entity);

            return eventService.RaisePrivilegeAddEventAsync(entity: entity);
        });

    public ValueTask RaisePrivilegeUpdateEventAsync(Privilege entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaisePrivilegeUpdateEvent(
                entity: entity);

            return eventService.RaisePrivilegeUpdateEventAsync(entity: entity);
        });

    public ValueTask RaisePrivilegeDeleteEventAsync(Privilege entity) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateRaisePrivilegeDeleteEvent(
                entity: entity);

            return eventService.RaisePrivilegeDeleteEventAsync(entity: entity);
        });
}