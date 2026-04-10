using cCoder.Data.Models.Security;
using EventLibrary.Models;

namespace cCoder.AppSecurity.Brokers.Events;

public interface IUserRoleEventBroker
{
    ValueTask RaiseUserRoleAddEventAsync(EventMessage<UserRole> message);
    ValueTask RaiseUserRoleDeleteEventAsync(EventMessage<UserRole> message);
}