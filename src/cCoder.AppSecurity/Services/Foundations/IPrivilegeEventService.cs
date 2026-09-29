// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal interface IPrivilegeEventService
{
    ValueTask RaisePrivilegeAddEventAsync(Privilege privilege);
    ValueTask RaisePrivilegeUpdateEventAsync(Privilege privilege);
    ValueTask RaisePrivilegeDeleteEventAsync(Privilege privilege);
}