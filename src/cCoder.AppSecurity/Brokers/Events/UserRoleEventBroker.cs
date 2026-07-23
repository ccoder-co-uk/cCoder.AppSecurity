// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.AppSecurity.Brokers.Events;

internal sealed class UserRoleEventBroker(IEventHub eventHub) : IUserRoleEventBroker
{
    public ValueTask RaiseUserRoleAddEventAsync(EventMessage<UserRole> message) =>
        eventHub.RaiseEventAsync(name: "user_role_add", message: message);

    public ValueTask RaiseUserRoleDeleteEventAsync(EventMessage<UserRole> message) =>
        eventHub.RaiseEventAsync(name: "user_role_delete", message: message);
}