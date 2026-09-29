// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

internal interface IRoleProcessingService
{
    Role Get(Guid id);
    IQueryable<Role> GetAll(bool ignoreFilters = false);
    ValueTask<Role> AddRoleAsync(Role entity);
    ValueTask<Role> AddValidatedRoleAsync(Role entity);
    ValueTask<Role> UpdateRoleAsync(Role entity);
    ValueTask<Role> UpdateValidatedRoleAsync(Role entity);
    ValueTask DeleteAsync(Guid id);
    ValueTask DeleteValidatedAsync(Guid id);
    ValueTask DeleteAllRoleAsync(IEnumerable<Role> items);
}