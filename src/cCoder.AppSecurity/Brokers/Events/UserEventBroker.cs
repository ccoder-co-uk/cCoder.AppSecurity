// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.AppSecurity.Brokers.Events;

internal sealed class UserEventBroker(IEventHub eventHub) : IUserEventBroker
{
    public ValueTask RaiseUserAddEventAsync(EventMessage<User> message) =>
        eventHub.RaiseEventAsync(name: "user_add", message: message);

    public ValueTask RaiseUserUpdateEventAsync(EventMessage<User> message) =>
        eventHub.RaiseEventAsync(name: "user_update", message: message);

    public ValueTask RaiseUserDeleteEventAsync(EventMessage<User> message) =>
        eventHub.RaiseEventAsync(name: "user_delete", message: message);
}