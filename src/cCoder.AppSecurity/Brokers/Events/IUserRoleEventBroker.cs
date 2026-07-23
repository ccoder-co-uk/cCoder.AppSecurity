// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;

namespace cCoder.AppSecurity.Brokers.Events;

public interface IUserRoleEventBroker
{
    ValueTask RaiseUserRoleAddEventAsync(EventMessage<UserRole> message);
    ValueTask RaiseUserRoleDeleteEventAsync(EventMessage<UserRole> message);
}