// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.AppSecurity.Brokers.Events;

public class UserEventBroker(IEventHub eventHub) : IUserEventBroker
{
    public ValueTask RaiseUserAddEventAsync(EventMessage<User> message) =>
        eventHub.RaiseEventAsync("user_add", message);

    public ValueTask RaiseUserUpdateEventAsync(EventMessage<User> message) =>
        eventHub.RaiseEventAsync("user_update", message);

    public ValueTask RaiseUserDeleteEventAsync(EventMessage<User> message) =>
        eventHub.RaiseEventAsync("user_delete", message);
}