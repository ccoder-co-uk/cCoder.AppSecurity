// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

public interface IPrivilegeProcessingService
{
    Privilege Get(string id);
    IQueryable<Privilege> GetAll(bool ignoreFilters = false);
    ValueTask<Privilege> AddPrivilegeAsync(Privilege entity);
    ValueTask<Privilege> UpdatePrivilegeAsync(Privilege entity);
    ValueTask DeleteAsync(string id);
    ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdatePrivilege(
        IEnumerable<Privilege> items
    );
    ValueTask DeleteAllPrivilegeAsync(IEnumerable<Privilege> items);
}