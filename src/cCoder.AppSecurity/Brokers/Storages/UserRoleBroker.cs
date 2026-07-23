// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;


namespace cCoder.AppSecurity.Brokers.Storages;

public interface IUserRoleBroker
{
    IQueryable<UserRole> GetAllUserRoles(bool ignoreFilters);
    ValueTask<UserRole> AddUserRoleAsync(UserRole entity);
    ValueTask<int> DeleteUserRoleAsync(UserRole entity);
    ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> items);
    int? GetAppId(UserRole entity);
}

internal sealed class UserRoleBroker(ICoreContextFactory coreContextFactory) : IUserRoleBroker
{

    public IQueryable<UserRole> GetAllUserRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return ignoreFilters
            ? coreDataContext.UserRoles.IgnoreQueryFilters()
            : coreDataContext.UserRoles;
    }

    public async ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        UserRole result = (await coreDataContext.UserRoles.AddAsync(entity: newUserRole)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteUserRoleAsync(UserRole deletedUserRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.UserRoles.Remove(entity: deletedUserRole);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> deletedUserRole)
    {
        if (deletedUserRole == null || !deletedUserRole.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.UserRoles.RemoveRange(entities: deletedUserRole);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(UserRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Roles

            .Where(predicate: role => role.Id == entity.RoleId)
            .Select(selector: role => (int?)role.AppId)
            .FirstOrDefault();

    }
}