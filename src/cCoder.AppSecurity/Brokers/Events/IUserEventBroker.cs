
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;

namespace cCoder.AppSecurity.Brokers.Events;

public interface IUserEventBroker
{
    ValueTask RaiseUserAddEventAsync(EventMessage<User> message);
    ValueTask RaiseUserUpdateEventAsync(EventMessage<User> message);
    ValueTask RaiseUserDeleteEventAsync(EventMessage<User> message);
}