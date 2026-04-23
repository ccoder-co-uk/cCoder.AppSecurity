using cCoder.Eventing.Models;
using DataPrivilege = cCoder.Data.Models.Security.Privilege;

namespace cCoder.AppSecurity.Brokers.Events;

public interface IPrivilegeEventBroker
{
    ValueTask RaisePrivilegeAddEventAsync(EventMessage<DataPrivilege> message);
    ValueTask RaisePrivilegeUpdateEventAsync(EventMessage<DataPrivilege> message);
    ValueTask RaisePrivilegeDeleteEventAsync(EventMessage<DataPrivilege> message);
}
