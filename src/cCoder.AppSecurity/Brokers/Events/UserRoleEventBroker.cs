// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.AppSecurity.Brokers.Events;

public class UserRoleEventBroker(IEventHub eventHub) : IUserRoleEventBroker
{
    public ValueTask RaiseUserRoleAddEventAsync(EventMessage<UserRole> message) =>
        eventHub.RaiseEventAsync("user_role_add", message);

    public ValueTask RaiseUserRoleDeleteEventAsync(EventMessage<UserRole> message) =>
        eventHub.RaiseEventAsync("user_role_delete", message);
}