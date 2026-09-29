// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

internal interface IPrivilegeService
{
    Privilege Get(string id);
    IQueryable<Privilege> GetAll(bool ignoreFilters = false);
    ValueTask<Privilege> AddPrivilegeAsync(Privilege privilege);
    ValueTask<Privilege> UpdatePrivilegeAsync(Privilege privilege);
    ValueTask DeleteAsync(string id);
}