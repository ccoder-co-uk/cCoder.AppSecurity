// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Exposures;

public interface IPrivilegeManager
{
    Privilege Get(string id);
    IQueryable<Privilege> GetAll(bool ignoreFilters = false);
    ValueTask<Privilege> AddPrivilegeAsync(Privilege entity);
    ValueTask<Privilege> UpdatePrivilegeAsync(Privilege entity);
    ValueTask DeleteAsync(string id);
    ValueTask DeleteAllPrivilegeAsync(IEnumerable<Privilege> items);
}
