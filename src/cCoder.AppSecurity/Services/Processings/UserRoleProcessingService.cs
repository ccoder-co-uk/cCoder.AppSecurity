// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class UserRoleProcessingService(
    IUserRoleFoundationService service
) : IUserRoleProcessingService
{
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

            Role role = service.GetRole(
                roleId: newUserRole.RoleId);

            User user = service.GetUser(
                userId: newUserRole.UserId);

            if (role == null || user == null || role.Users?.Any(predicate: u => u.UserId == user.Id) == true)
            {
                throw new SecurityException(message: "Access Denied!");
            }

            service.Authorize(
                appId: role.AppId,
                privilege: "userrole_create");

            if (role.Privileges.Contains(item: "app_admin")
                && !service.IsAdminOfApp(appId: role.AppId))
            {
                throw new SecurityException(message: "Access Denied!");
            }

            return await service.AddUserRoleAsync(
                newUserRole: newUserRole);

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
                newUserRole: entity,
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

            if (dbVersion == null || GetCurrentUserId() == null)
            {
                throw new SecurityException(message: "Access Denied!");
            }

            int appId = service.GetRole(
                roleId: dbVersion.RoleId)
                .AppId;

            service.Authorize(
                appId: appId,
                privilege: "userrole_delete");

            await service.DeleteUserRoleAsync(
                deletedUserRole: dbVersion);

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

    private IQueryable<UserRole> GetAllValue(bool ignoreFilters = false) =>
        GetAll(ignoreFilters: ignoreFilters);

    private ValueTask<UserRole> AddUserRoleValueAsync(UserRole newUserRole) =>
        AddUserRoleAsync(newUserRole: newUserRole);

    private ValueTask DeleteUserRoleValueAsync(UserRole deletedUserRole) =>
        DeleteUserRoleAsync(deletedUserRole: deletedUserRole);

    private ValueTask DeleteAllUserRoleValueAsync(IEnumerable<UserRole> deletedUserRole) =>
        DeleteAllUserRoleAsync(deletedUserRole: deletedUserRole);

    private string GetCurrentUserId() =>
        service.GetCurrentUser()?.Id;
}