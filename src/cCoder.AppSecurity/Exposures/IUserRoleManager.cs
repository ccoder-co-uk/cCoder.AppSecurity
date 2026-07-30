// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Exposures;

public interface IUserRoleManager
{
    IQueryable<UserRole> GetAll(bool ignoreFilters = false);
    ValueTask<UserRole> AddUserRoleAsync(UserRole entity);
    ValueTask DeleteUserRoleAsync(UserRole entity);
    ValueTask DeleteAllUserRoleAsync(IEnumerable<UserRole> items);
    ValueTask<UserRole> SaveUserRoleAsync(UserRole entity);
}
