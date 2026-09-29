// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

internal interface IPrivilegeEventProcessingService
{
    ValueTask RaisePrivilegeAddEventAsync(Privilege privilege);
    ValueTask RaisePrivilegeUpdateEventAsync(Privilege privilege);
    ValueTask RaisePrivilegeDeleteEventAsync(Privilege privilege);
}