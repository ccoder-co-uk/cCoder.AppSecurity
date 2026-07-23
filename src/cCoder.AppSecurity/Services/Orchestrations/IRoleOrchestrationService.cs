// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Orchestrations;

public interface IRoleOrchestrationService
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