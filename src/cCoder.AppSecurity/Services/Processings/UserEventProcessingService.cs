// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Services.Processings;

internal class UserEventProcessingService(IUserEventService eventService) : IUserEventProcessingService
{
    public ValueTask RaiseUserAddEventAsync(User entity) => eventService.RaiseUserAddEventAsync(entity: entity);

    public ValueTask RaiseUserUpdateEventAsync(User entity) => eventService.RaiseUserUpdateEventAsync(entity: entity);

    public ValueTask RaiseUserDeleteEventAsync(User entity) => eventService.RaiseUserDeleteEventAsync(entity: entity);
}