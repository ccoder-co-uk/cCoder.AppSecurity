// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using DataRole = cCoder.Data.Models.Security.Role;


namespace cCoder.AppSecurity.Brokers.Events;

internal sealed class RoleEventBroker(IEventHub eventHub) : IRoleEventBroker
{
    public ValueTask RaiseRoleAddEventAsync(EventMessage<DataRole> message) =>
        eventHub.RaiseEventAsync(name: "role_add", message: message);

    public ValueTask RaiseRoleUpdateEventAsync(EventMessage<DataRole> message) =>
        eventHub.RaiseEventAsync(name: "role_update", message: message);

    public ValueTask RaiseRoleDeleteEventAsync(EventMessage<DataRole> message) =>
        eventHub.RaiseEventAsync(name: "role_delete", message: message);
}