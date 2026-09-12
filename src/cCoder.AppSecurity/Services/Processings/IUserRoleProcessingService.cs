// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

internal interface IUserRoleProcessingService
{
    IQueryable<UserRole> GetAll(bool ignoreFilters = false);
    ValueTask<UserRole> AddUserRoleAsync(UserRole userRole);
    ValueTask DeleteUserRoleAsync(UserRole userRole);
    ValueTask DeleteAllUserRoleAsync(IEnumerable<UserRole> items);
    ValueTask<UserRole> SaveUserRoleAsync(UserRole userRole);
}