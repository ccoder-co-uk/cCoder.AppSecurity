// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Services.Processings;

internal class RoleEventProcessingService(IRoleEventService eventService) : IRoleEventProcessingService
{
    public ValueTask RaiseRoleAddEventAsync(Role entity) => eventService.RaiseRoleAddEventAsync(entity: entity);

    public ValueTask RaiseRoleUpdateEventAsync(Role entity) => eventService.RaiseRoleUpdateEventAsync(entity: entity);

    public ValueTask RaiseRoleDeleteEventAsync(Role entity) => eventService.RaiseRoleDeleteEventAsync(entity: entity);
}