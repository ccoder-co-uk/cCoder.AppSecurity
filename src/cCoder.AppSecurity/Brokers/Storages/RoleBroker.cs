// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.AppSecurity.Brokers;

public interface IRoleBroker
{
    IQueryable<Role> GetAllRoles(bool ignoreFilters);
    ValueTask<Role> AddRoleAsync(Role entity);
    ValueTask<Role> UpdateRoleAsync(Role entity);
    ValueTask DeleteFolderRolesByRoleIdAsync(Guid roleId);
    ValueTask DeletePageRolesByRoleIdAsync(Guid roleId);
    ValueTask<int> DeleteRoleAsync(Role entity);
    ValueTask DeleteAllRolesAsync(IEnumerable<Role> items);
    int? GetAppId(Role entity);
}

internal sealed class RoleBroker(ICoreContextFactory coreContextFactory) : IRoleBroker
{

    public IQueryable<Role> GetAllRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Dictionary<bool, Func<IQueryable<Role>>> queries = new()
        {
            [false] = () => coreDataContext.Roles,
            [true] = () => coreDataContext.Roles.IgnoreQueryFilters(),
        };

        return queries[ignoreFilters]();
    }

    public async ValueTask<Role> AddRoleAsync(Role newRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = (await coreDataContext.Roles.AddAsync(entity: newRole)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Role> UpdateRoleAsync(Role updatedRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = coreDataContext.Roles.Update(entity: updatedRole).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask DeleteFolderRolesByRoleIdAsync(Guid roleId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        FolderRole[] folderRoles = [.. coreDataContext.FolderRoles
            .IgnoreQueryFilters()
            .Where(predicate: folderRole => folderRole.RoleId == roleId)];

        coreDataContext.FolderRoles.RemoveRange(entities: folderRoles);
        await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeletePageRolesByRoleIdAsync(Guid roleId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        PageRole[] pageRoles = [.. coreDataContext.PageRoles
            .IgnoreQueryFilters()
            .Where(predicate: pageRole => pageRole.RoleId == roleId)];

        coreDataContext.PageRoles.RemoveRange(entities: pageRoles);
        await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask<int> DeleteRoleAsync(Role deletedRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.Remove(entity: deletedRole);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllRolesAsync(IEnumerable<Role> deletedRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.RemoveRange(entities: deletedRole);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(Role entity)
    {
        return entity.AppId;
    }
}