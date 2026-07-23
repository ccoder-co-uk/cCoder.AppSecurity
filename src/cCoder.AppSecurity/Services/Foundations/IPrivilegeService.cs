// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

public interface IPrivilegeService
{
    Privilege Get(string id);
    IQueryable<Privilege> GetAll(bool ignoreFilters = false);
    ValueTask<Privilege> AddPrivilegeAsync(Privilege privilege);
    ValueTask<Privilege> UpdatePrivilegeAsync(Privilege privilege);
    ValueTask DeleteAsync(string id);
}