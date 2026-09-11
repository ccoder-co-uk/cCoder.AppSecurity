// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

internal interface IPrivilegeEventProcessingService
{
    ValueTask RaisePrivilegeAddEventAsync(Privilege privilege);
    ValueTask RaisePrivilegeUpdateEventAsync(Privilege privilege);
    ValueTask RaisePrivilegeDeleteEventAsync(Privilege privilege);
}