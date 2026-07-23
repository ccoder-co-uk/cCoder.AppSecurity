// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Foundations;

internal interface IAuthorizationService
{
    User GetCurrentUser();
}