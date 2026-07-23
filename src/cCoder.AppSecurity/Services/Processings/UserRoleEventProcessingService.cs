// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Services.Processings;

internal class UserRoleEventProcessingService(IUserRoleEventService eventService) : IUserRoleEventProcessingService
{
    public ValueTask RaiseUserRoleAddEventAsync(UserRole entity) => eventService.RaiseUserRoleAddEventAsync(entity: entity);

    public ValueTask RaiseUserRoleDeleteEventAsync(UserRole entity) => eventService.RaiseUserRoleDeleteEventAsync(entity: entity);
}