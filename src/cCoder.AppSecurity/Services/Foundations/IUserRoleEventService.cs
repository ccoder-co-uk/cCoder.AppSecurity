// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal interface IUserRoleEventService
{
    ValueTask RaiseUserRoleAddEventAsync(UserRole userRole);
    ValueTask RaiseUserRoleDeleteEventAsync(UserRole userRole);
}