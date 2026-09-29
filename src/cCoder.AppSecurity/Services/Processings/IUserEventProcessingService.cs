// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

internal interface IUserEventProcessingService
{
    ValueTask RaiseUserAddEventAsync(User user);
    ValueTask RaiseUserUpdateEventAsync(User user);
    ValueTask RaiseUserDeleteEventAsync(User user);
}