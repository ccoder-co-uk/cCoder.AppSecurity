// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using DataRole = cCoder.Data.Models.Security.Role;


namespace cCoder.AppSecurity.Brokers.Events;

public class RoleEventBroker(IEventHub eventHub) : IRoleEventBroker
{
    public ValueTask RaiseRoleAddEventAsync(EventMessage<DataRole> message) =>
        eventHub.RaiseEventAsync("role_add", message);

    public ValueTask RaiseRoleUpdateEventAsync(EventMessage<DataRole> message) =>
        eventHub.RaiseEventAsync("role_update", message);

    public ValueTask RaiseRoleDeleteEventAsync(EventMessage<DataRole> message) =>
        eventHub.RaiseEventAsync("role_delete", message);
}