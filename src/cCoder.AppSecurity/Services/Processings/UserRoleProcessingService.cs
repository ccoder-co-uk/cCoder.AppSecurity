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

internal sealed partial class UserRoleProcessingService(
    IUserRoleService service,
    IRoleService roleService,
    IUserService userService,
    IAuthorizationBroker authorizationBroker
) : IUserRoleProcessingService
{
    private string CurrentUserId => authorizationBroker.GetCurrentUser()?.Id;

    public IQueryable<UserRole> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<UserRole> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return service.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole) =>
        TryCatch(operation: async ValueTask<UserRole> () =>
        {
            ValidateAddUserRole(
                newUserRole: newUserRole);

            Role role = roleService
                .GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: r => r.Id == newUserRole.RoleId);

            User user = userService
                .GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: u => u.Id == newUserRole.UserId);

            if (role == null || user == null || role.Users?.Any(predicate: u => u.UserId == user.Id) == true)
            {
                throw new SecurityException(message: "Access Denied!");
            }

            authorizationBroker.Authorize(appId: role.AppId, privilege: "userrole_create");

            if (role.Privileges.Contains(item: "app_admin") && !authorizationBroker.IsAdminOfApp(appId: role.AppId))
            {
                throw new SecurityException(message: "Access Denied!");
            }

            return await service.AddUserRoleAsync(userRole: newUserRole);

        });

    public ValueTask<UserRole> SaveUserRoleAsync(UserRole entity) =>
        TryCatch(operation: async ValueTask<UserRole> () =>
        {
            ValidateSaveUserRole(
                entity: entity);

            UserRole existingUserRole = service
                .GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: userRole =>
                    userRole.UserId == entity.UserId
                    && userRole.RoleId == entity.RoleId);

            if (existingUserRole != null)
            {
                return existingUserRole;
            }

            return await service.AddUserRoleAsync(
                userRole: entity,
                authorize: false);

        });

    public ValueTask DeleteUserRoleAsync(UserRole deletedUserRole) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDeleteUserRole(
                deletedUserRole: deletedUserRole);

            UserRole dbVersion = service
                .GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: ur => ur.RoleId == deletedUserRole.RoleId && ur.UserId == deletedUserRole.UserId);

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
            await service.DeleteUserRoleAsync(userRole: dbVersion);

        });

    public ValueTask DeleteAllUserRoleAsync(IEnumerable<UserRole> deletedUserRole) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDeleteAllUserRole(
                deletedUserRole: deletedUserRole);

            foreach (UserRole item in deletedUserRole)
            {
                await DeleteUserRoleValueAsync(deletedUserRole: item);
            }

        });

    public ValueTask<IEnumerable<Result<UserRole>>> AddOrUpdateUserRole(
        IEnumerable<UserRole> items
    ) =>
        TryCatch(operation: async ValueTask<IEnumerable<Result<UserRole>>> () =>
        {
            ValidateAddOrUpdateUserRole(
                items: items);

            UserRole[] itemArray = [.. items];

            var leftIds = itemArray.Select(selector: item => item.UserId)
                .Distinct()
                .ToArray();

            UserRole[] existingItems = [.. GetAllValue()
                .Where(predicate: item => leftIds.Contains(value: item.UserId))];

            List<Result<UserRole>> results = [];

            foreach (var group in itemArray.GroupBy(keySelector: item => item.UserId))
            {
                UserRole[] groupItems = [.. group];

                UserRole[] existingGroupItems =
                [
                    .. existingItems.Where(predicate: item => Equals(objA: item.UserId, objB: group.Key)),
                ];

                await DeleteAllUserRoleValueAsync(deletedUserRole: existingGroupItems);

                foreach (UserRole item in groupItems)
                {
                    try
                    {
                        results.Add(
    item: new Result<UserRole>
    {
        Id = $"{item.UserId}:{item.RoleId}",
        Success = true,
        Item = await AddUserRoleValueAsync(newUserRole: item),
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

        });

    private IQueryable<UserRole> GetAllValue(bool ignoreFilters = false) =>
        GetAll(ignoreFilters: ignoreFilters);

    private ValueTask<UserRole> AddUserRoleValueAsync(UserRole newUserRole) =>
        AddUserRoleAsync(newUserRole: newUserRole);

    private ValueTask DeleteUserRoleValueAsync(UserRole deletedUserRole) =>
        DeleteUserRoleAsync(deletedUserRole: deletedUserRole);

    private ValueTask DeleteAllUserRoleValueAsync(IEnumerable<UserRole> deletedUserRole) =>
        DeleteAllUserRoleAsync(deletedUserRole: deletedUserRole);
}