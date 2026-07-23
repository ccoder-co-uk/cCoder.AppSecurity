// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.AppSecurity.Brokers;

public interface IPrivilegeBroker
{
    IQueryable<Privilege> GetAllPrivileges(bool ignoreFilters);
    ValueTask<Privilege> AddPrivilegeAsync(Privilege entity);
    ValueTask<Privilege> UpdatePrivilegeAsync(Privilege entity);
    ValueTask<int> DeletePrivilegeAsync(Privilege entity);
    ValueTask DeleteAllPrivilegesAsync(IEnumerable<Privilege> items);
    int? GetAppId(Privilege entity);
}

internal sealed class PrivilegeBroker(ICoreContextFactory coreContextFactory) : IPrivilegeBroker
{

    public IQueryable<Privilege> GetAllPrivileges(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.Set<Privilege>()
                .IgnoreQueryFilters()
            : coreDataContext.Set<Privilege>();
    }

    public async ValueTask<Privilege> AddPrivilegeAsync(Privilege newPrivilege)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Privilege result = (await coreDataContext.Set<Privilege>()
            .AddAsync(entity: newPrivilege))
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Privilege> UpdatePrivilegeAsync(Privilege updatedPrivilege)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Privilege result = coreDataContext.Set<Privilege>()
            .Update(entity: updatedPrivilege)
            .Entity;

        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePrivilegeAsync(Privilege deletedPrivilege)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        coreDataContext.Set<Privilege>()
            .Remove(entity: deletedPrivilege);

        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPrivilegesAsync(IEnumerable<Privilege> deletedPrivilege)
    {
        if (deletedPrivilege == null || !deletedPrivilege.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        coreDataContext.Set<Privilege>()
            .RemoveRange(entities: deletedPrivilege);

        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(Privilege entity)
    {
        return null;
    }
}