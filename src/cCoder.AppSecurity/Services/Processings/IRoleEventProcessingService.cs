// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

internal interface IRoleEventProcessingService
{
    ValueTask RaiseRoleAddEventAsync(Role role);
    ValueTask RaiseRoleUpdateEventAsync(Role role);
    ValueTask RaiseRoleDeleteEventAsync(Role role);
}