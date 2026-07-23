// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Foundations;

internal interface IUserRoleFoundationService : IUserRoleService
{
    Role GetRole(Guid roleId);

    User GetUser(string userId);

    User GetCurrentUser();

    void Authorize(int? appId, string privilege);

    bool IsAdminOfApp(int? appId);
}