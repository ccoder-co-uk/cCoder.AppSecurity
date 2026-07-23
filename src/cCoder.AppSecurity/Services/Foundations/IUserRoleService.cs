// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

public interface IUserRoleService
{
    IQueryable<UserRole> GetAll(bool ignoreFilters = false);
    ValueTask<UserRole> AddAsync(UserRole userRole, bool authorize = true);
    ValueTask DeleteAsync(UserRole userRole);
}