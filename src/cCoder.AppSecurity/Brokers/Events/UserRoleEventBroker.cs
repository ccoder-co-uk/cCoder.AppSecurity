using cCoder.Data.Models.Security;
using EventLibrary;
using EventLibrary.Models;


namespace cCoder.AppSecurity.Brokers.Events;

public class UserRoleEventBroker(IEventHub eventHub) : IUserRoleEventBroker
{
    public ValueTask RaiseUserRoleAddEventAsync(EventMessage<UserRole> message) =>
        eventHub.RaiseEventAsync("user_role_add", message);

    public ValueTask RaiseUserRoleDeleteEventAsync(EventMessage<UserRole> message) =>
        eventHub.RaiseEventAsync("user_role_delete", message);
}