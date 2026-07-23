// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

public interface IPrivilegeEventProcessingService
{
    ValueTask RaisePrivilegeAddEventAsync(Privilege entity);
    ValueTask RaisePrivilegeUpdateEventAsync(Privilege entity);
    ValueTask RaisePrivilegeDeleteEventAsync(Privilege entity);
}