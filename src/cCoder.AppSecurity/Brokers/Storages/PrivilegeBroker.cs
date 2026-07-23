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
            ? coreDataContext.Set<Privilege>().IgnoreQueryFilters()
            : coreDataContext.Set<Privilege>();
    }

    public async ValueTask<Privilege> AddPrivilegeAsync(Privilege entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Privilege result = (await coreDataContext.Set<Privilege>().AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Privilege> UpdatePrivilegeAsync(Privilege entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Privilege result = coreDataContext.Set<Privilege>().Update(entity: entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeletePrivilegeAsync(Privilege entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Set<Privilege>().Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllPrivilegesAsync(IEnumerable<Privilege> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Set<Privilege>().RemoveRange(entities: items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(Privilege entity)
    {
        return null;
    }
}