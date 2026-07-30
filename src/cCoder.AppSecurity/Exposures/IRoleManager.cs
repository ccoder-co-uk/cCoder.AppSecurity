// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Exposures;

public interface IRoleManager
{
    Role Get(Guid id);
    IQueryable<Role> GetAll(bool ignoreFilters = false);
    ValueTask<Role> AddRoleAsync(Role entity);
    ValueTask<Role> AddValidatedRoleAsync(Role entity);
    ValueTask<Role> UpdateRoleAsync(Role entity);
    ValueTask<Role> UpdateValidatedRoleAsync(Role entity);
    ValueTask DeleteAsync(Guid id);
    ValueTask DeleteValidatedAsync(Guid id);
    ValueTask ImportRoleAsync(int appId, IEnumerable<Role> roles);
    ValueTask DeleteAllRoleAsync(IEnumerable<Role> items);
}
