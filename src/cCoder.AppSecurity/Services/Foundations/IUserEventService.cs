// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal interface IUserEventService
{
    ValueTask RaiseUserAddEventAsync(User user);
    ValueTask RaiseUserUpdateEventAsync(User user);
    ValueTask RaiseUserDeleteEventAsync(User user);
}