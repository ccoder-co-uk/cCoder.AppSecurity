// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Orchestrations;

public interface IUserRoleOrchestrationService
{
    IQueryable<UserRole> GetAll(bool ignoreFilters = false);
    ValueTask<UserRole> AddUserRoleAsync(UserRole entity);
    ValueTask DeleteUserRoleAsync(UserRole entity);
    ValueTask DeleteAllUserRoleAsync(IEnumerable<UserRole> items);
    ValueTask<UserRole> SaveUserRoleAsync(UserRole entity);
}