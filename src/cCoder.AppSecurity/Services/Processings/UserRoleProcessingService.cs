// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;


namespace cCoder.AppSecurity.Services.Processings;

internal class UserRoleProcessingService(
    IUserRoleService service,
    IRoleService roleService,
    IUserService userService,
    IAuthorizationBroker authorizationBroker
) : IUserRoleProcessingService
{
    private string CurrentUserId => authorizationBroker.GetCurrentUser()?.Id;

    public IQueryable<UserRole> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<UserRole> AddAsync(UserRole entity)
    {
        Role role = roleService
            .GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: r => r.Id == entity.RoleId);

        User user = userService
            .GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: u => u.Id == entity.UserId);

        if (role == null || user == null || role.Users?.Any(predicate: u => u.UserId == user.Id) == true)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        authorizationBroker.Authorize(appId: role.AppId, privilege: "userrole_create");

        if (role.Privileges.Contains(item: "app_admin") && !authorizationBroker.IsAdminOfApp(appId: role.AppId))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return await service.AddAsync(userRole: entity);
    }

    public async ValueTask<UserRole> SaveAsync(UserRole entity)
    {
        UserRole existingUserRole = service
            .GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: userRole =>
                userRole.UserId == entity.UserId
                && userRole.RoleId == entity.RoleId);

        if (existingUserRole != null)
        {
            return existingUserRole;
        }

        return await service.AddAsync(entity, authorize: false);
    }

    public async ValueTask DeleteAsync(UserRole link)
    {
        UserRole dbVersion = service
            .GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: ur => ur.RoleId == link.RoleId && ur.UserId == link.UserId);

        if (dbVersion == null || CurrentUserId == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        int appId = roleService
            .GetAll(ignoreFilters: true)
            .Where(predicate: role => role.Id == dbVersion.RoleId)
            .Select(selector: role => role.AppId)
            .FirstOrDefault();

        authorizationBroker.Authorize(appId: appId, privilege: "userrole_delete");
        await service.DeleteAsync(userRole: dbVersion);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<UserRole> items)
    {
        foreach (UserRole item in items)
        {
            await DeleteAsync(link: item);
        }
    }

    public async ValueTask<IEnumerable<Result<UserRole>>> AddOrUpdate(
        IEnumerable<UserRole> items
    )
    {
        UserRole[] itemArray = [.. items];
        var leftIds = itemArray.Select(selector: item => item.UserId).Distinct().ToArray();
        UserRole[] existingItems = [.. GetAll().Where(predicate: item => leftIds.Contains(value: item.UserId))];

        List<Result<UserRole>> results = [];
        foreach (var group in itemArray.GroupBy(keySelector: item => item.UserId))
        {
            UserRole[] groupItems = [.. group];
            UserRole[] existingGroupItems =
            [
                .. existingItems.Where(predicate: item => Equals(objA: item.UserId, objB: group.Key)),
            ];

            await DeleteAllAsync(items: existingGroupItems);
            foreach (UserRole item in groupItems)
            {
                try
                {
                    results.Add(
item: new Result<UserRole>
{
    Id = $"{item.UserId}:{item.RoleId}",
    Success = true,
    Item = await AddAsync(entity: item),
    Message = "Added Successfully",
}
                    );
                }
                catch (Exception ex)
                {
                    results.Add(
item: new Result<UserRole>
{
    Id = $"{item.UserId}:{item.RoleId}",
    Success = false,
    Item = item,
    Message = ex.Message,
}
                    );
                }
            }
        }

        return results;
    }
}