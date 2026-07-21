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

    public IQueryable<UserRole> GetAll(bool ignoreFilters = false) => service.GetAll(ignoreFilters);

    public async ValueTask<UserRole> AddAsync(UserRole entity)
    {
        Role role = roleService
            .GetAll(true)
            .FirstOrDefault(r => r.Id == entity.RoleId);

        User user = userService
            .GetAll(true)
            .FirstOrDefault(u => u.Id == entity.UserId);

        if (role == null || user == null || role.Users?.Any(u => u.UserId == user.Id) == true)
            throw new SecurityException("Access Denied!");

        authorizationBroker.Authorize(role.AppId, "userrole_create");

        if (role.Privileges.Contains("app_admin") && !authorizationBroker.IsAdminOfApp(role.AppId))
            throw new SecurityException("Access Denied!");

        return await service.AddAsync(entity);
    }

    public async ValueTask<UserRole> SaveAsync(UserRole entity)
    {
        UserRole existingUserRole = service
            .GetAll(true)
            .FirstOrDefault(userRole =>
                userRole.UserId == entity.UserId
                && userRole.RoleId == entity.RoleId);

        if (existingUserRole != null)
            return existingUserRole;

        return await service.AddAsync(entity);
    }

    public async ValueTask DeleteAsync(UserRole link)
    {
        UserRole dbVersion = service
            .GetAll(true)
            .FirstOrDefault(ur => ur.RoleId == link.RoleId && ur.UserId == link.UserId);

        if (dbVersion == null || CurrentUserId == null)
            throw new SecurityException("Access Denied!");

        int appId = roleService
            .GetAll(true)
            .Where(role => role.Id == dbVersion.RoleId)
            .Select(role => role.AppId)
            .FirstOrDefault();

        authorizationBroker.Authorize(appId, "userrole_delete");
        await service.DeleteAsync(dbVersion);
    }

    public async ValueTask DeleteAllAsync(IEnumerable<UserRole> items)
    {
        foreach (UserRole item in items)
        {
            await DeleteAsync(item);
        }
    }

    public async ValueTask<IEnumerable<Result<UserRole>>> AddOrUpdate(
        IEnumerable<UserRole> items
    )
    {
        UserRole[] itemArray = [.. items];
        var leftIds = itemArray.Select(item => item.UserId).Distinct().ToArray();
        UserRole[] existingItems = [.. GetAll().Where(item => leftIds.Contains(item.UserId))];

        List<Result<UserRole>> results = [];
        foreach (var group in itemArray.GroupBy(item => item.UserId))
        {
            UserRole[] groupItems = [.. group];
            UserRole[] existingGroupItems =
            [
                .. existingItems.Where(item => Equals(item.UserId, group.Key)),
            ];

            await DeleteAllAsync(existingGroupItems);
            foreach (UserRole item in groupItems)
            {
                try
                {
                    results.Add(
                        new Result<UserRole>
                        {
                            Id = $"{item.UserId}:{item.RoleId}",
                            Success = true,
                            Item = await AddAsync(item),
                            Message = "Added Successfully",
                        }
                    );
                }
                catch (Exception ex)
                {
                    results.Add(
                        new Result<UserRole>
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














