// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Services.Processings;

internal class PrivilegeEventProcessingService(IPrivilegeEventService eventService) : IPrivilegeEventProcessingService
{
    public ValueTask RaisePrivilegeAddEventAsync(Privilege entity) =>
        eventService.RaisePrivilegeAddEventAsync(entity: entity);

    public ValueTask RaisePrivilegeUpdateEventAsync(Privilege entity) =>
        eventService.RaisePrivilegeUpdateEventAsync(entity: entity);

    public ValueTask RaisePrivilegeDeleteEventAsync(Privilege entity) =>
        eventService.RaisePrivilegeDeleteEventAsync(entity: entity);
}