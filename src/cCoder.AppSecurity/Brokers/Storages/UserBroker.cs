// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.AppSecurity.Brokers.Storages;

public interface IUserBroker
{
    IQueryable<User> GetAllUsers(bool ignoreFilters);
    User GetUserByEmail(string email, bool ignoreFilters);
    ValueTask<User> AddUserAsync(User entity);
    ValueTask<User> UpdateUserAsync(User entity);
    ValueTask<int> DeleteUserAsync(User entity);
    ValueTask DeleteAllUsersAsync(IEnumerable<User> items);
    int? GetAppId(User entity);
}

internal sealed class UserBroker(ICoreContextFactory coreContextFactory) : IUserBroker
{

    public IQueryable<User> GetAllUsers(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Dictionary<bool, Func<IQueryable<User>>> queries = new()
        {
            [false] = () => coreDataContext.Users,
            [true] = () => coreDataContext.Users.IgnoreQueryFilters(),
        };

        return queries[ignoreFilters]();
    }

    public User GetUserByEmail(string email, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Dictionary<bool, Func<IQueryable<User>>> queries = new()
        {
            [false] = () => coreDataContext.Users,
            [true] = () => coreDataContext.Users.IgnoreQueryFilters(),
        };

        IQueryable<User> users = queries[ignoreFilters]();

        return users.FirstOrDefault(predicate: user => user.Email == email);
    }

    public async ValueTask<User> AddUserAsync(User newUser)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        User result = (await coreDataContext.Users.AddAsync(entity: newUser)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<User> UpdateUserAsync(User updatedUser)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        User result = coreDataContext.Users.Update(entity: updatedUser).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteUserAsync(User deletedUser)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Users.Remove(entity: deletedUser);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllUsersAsync(IEnumerable<User> deletedUser)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Users.RemoveRange(entities: deletedUser);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(User entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.UserRoles

            .Where(predicate: userRole => userRole.UserId == entity.Id)
            .Join(inner: coreDataContext.Roles,
outerKeySelector: userRole => userRole.RoleId,
innerKeySelector: role => role.Id,
resultSelector: (userRole, role) => (int?)role.AppId)
            .FirstOrDefault();

    }
}