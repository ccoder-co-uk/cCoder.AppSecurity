// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using DataRole = cCoder.Data.Models.Security.Role;

namespace cCoder.AppSecurity.Brokers.Events;

public interface IRoleEventBroker
{
    ValueTask RaiseRoleAddEventAsync(EventMessage<DataRole> message);
    ValueTask RaiseRoleUpdateEventAsync(EventMessage<DataRole> message);
    ValueTask RaiseRoleDeleteEventAsync(EventMessage<DataRole> message);
}