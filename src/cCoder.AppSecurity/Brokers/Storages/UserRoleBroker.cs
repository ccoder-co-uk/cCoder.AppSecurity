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

public class UserRoleBroker(ICoreContextFactory coreContextFactory) : IUserRoleBroker
{

    public IQueryable<UserRole> GetAllUserRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.UserRoles.IgnoreQueryFilters()
            : coreDataContext.UserRoles;
    }

    public async ValueTask<UserRole> AddUserRoleAsync(UserRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        UserRole result = (await coreDataContext.UserRoles.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteUserRoleAsync(UserRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.UserRoles.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.UserRoles.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(UserRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.Roles
            .IgnoreQueryFilters()

            .Where(role => role.Id == entity.RoleId)
            .Select(role => (int?)role.AppId)
            .FirstOrDefault();

    }
}







