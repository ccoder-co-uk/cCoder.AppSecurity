// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Processings;

internal interface IAccountRoleAssignmentProcessingService
{
    ValueTask AttachUsersRoleAsync(
        User user,
        int appId);
}