using EventLibrary;
using EventLibrary.Models;
using DataPrivilege = cCoder.Data.Models.Security.Privilege;

namespace cCoder.AppSecurity.Brokers.Events;

public class PrivilegeEventBroker(IEventHub eventHub) : IPrivilegeEventBroker
{
    public ValueTask RaisePrivilegeAddEventAsync(EventMessage<DataPrivilege> message) =>
        eventHub.RaiseEventAsync("privilege_add", message);

    public ValueTask RaisePrivilegeUpdateEventAsync(EventMessage<DataPrivilege> message) =>
        eventHub.RaiseEventAsync("privilege_update", message);

    public ValueTask RaisePrivilegeDeleteEventAsync(EventMessage<DataPrivilege> message) =>
        eventHub.RaiseEventAsync("privilege_delete", message);
}
