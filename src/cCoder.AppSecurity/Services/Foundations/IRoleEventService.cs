// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal interface IRoleEventService
{
    ValueTask RaiseRoleAddEventAsync(Role role);
    ValueTask RaiseRoleUpdateEventAsync(Role role);
    ValueTask RaiseRoleDeleteEventAsync(Role role);
}