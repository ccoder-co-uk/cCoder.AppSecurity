// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

internal interface IUserRoleService
{
    IQueryable<UserRole> GetAll(bool ignoreFilters = false);
    ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole, bool authorize = true);

    ValueTask DeleteUserRoleAsync(UserRole deletedUserRole);
}