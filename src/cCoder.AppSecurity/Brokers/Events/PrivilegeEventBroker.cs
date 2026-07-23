// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using DataPrivilege = cCoder.Data.Models.Security.Privilege;

namespace cCoder.AppSecurity.Brokers.Events;

internal sealed class PrivilegeEventBroker(IEventHub eventHub) : IPrivilegeEventBroker
{
    public ValueTask RaisePrivilegeAddEventAsync(EventMessage<DataPrivilege> message) =>
        eventHub.RaiseEventAsync(name: "privilege_add", message: message);

    public ValueTask RaisePrivilegeUpdateEventAsync(EventMessage<DataPrivilege> message) =>
        eventHub.RaiseEventAsync(name: "privilege_update", message: message);

    public ValueTask RaisePrivilegeDeleteEventAsync(EventMessage<DataPrivilege> message) =>
        eventHub.RaiseEventAsync(name: "privilege_delete", message: message);
}