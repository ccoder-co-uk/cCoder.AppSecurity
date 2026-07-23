// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IRoleBroker = cCoder.AppSecurity.Brokers.IRoleBroker;
using IUserRoleBroker = cCoder.AppSecurity.Brokers.Storages.IUserRoleBroker;


namespace cCoder.AppSecurity.Services.Foundations;

internal class UserRoleService(
    IUserRoleBroker userRoleBroker,
    IRoleBroker roleBroker,
    IAuthorizationBroker authorizationBroker
) : IUserRoleService
{
    public IQueryable<UserRole> GetAll(bool ignoreFilters = false) =>
        userRoleBroker.GetAllUserRoles(ignoreFilters: ignoreFilters);

    public async ValueTask<UserRole> AddUserRoleAsync(UserRole userRole, bool authorize = true)
    {
        DataUserRole internalUserRole = new()
        {
            RoleId = userRole.RoleId,
            UserId = userRole.UserId
        };

        if (authorize)
        {
            int? appId = userRoleBroker.GetAppId(entity: internalUserRole);
            authorizationBroker.Authorize(appId: appId, privilege: $"{nameof(UserRole)}_create");
            AuthorizeAssignedRolePrivileges(appId: appId, roleId: userRole.RoleId);
        }

        DataUserRole result = await userRoleBroker.AddUserRoleAsync(entity: internalUserRole);
        userRole.RoleId = result.RoleId;
        userRole.UserId = result.UserId;
        return userRole;
    }

    public async ValueTask DeleteUserRoleAsync(UserRole userRole)
    {
        DataUserRole internalUserRole = ToExternalUserRole(item: userRole);

        authorizationBroker.Authorize(
appId: userRoleBroker.GetAppId(entity: internalUserRole),
privilege: $"{nameof(UserRole)}_delete"
        );

        _ = await userRoleBroker.DeleteUserRoleAsync(entity: internalUserRole);
    }

    private void AuthorizeAssignedRolePrivileges(int? appId, Guid roleId)
    {
        if (!appId.HasValue)
        {
            return;
        }

        Role role = roleBroker.GetAllRoles(ignoreFilters: true)
            .FirstOrDefault(predicate: foundRole => foundRole.Id == roleId);

        if (role is null)
        {
            return;
        }

        string[] assignedPrivilegeSet = ToPrivilegeSet(privileges: role.Privs);

        if (assignedPrivilegeSet.Length == 0)
        {
            return;
        }

        User currentUser = authorizationBroker.GetCurrentUser();

        HashSet<string> grantedPrivileges = currentUser?.Roles?
            .Where(predicate: userRole => userRole.Role?.AppId == appId.Value)
            .SelectMany(selector: userRole => ToPrivilegeSet(privileges: userRole.Role.Privs))
            .ToHashSet(comparer: StringComparer.OrdinalIgnoreCase)
            ?? [];

        if (assignedPrivilegeSet.Any(predicate: assignedPrivilege => !grantedPrivileges.Contains(item: assignedPrivilege)))
        {
            throw new System.Security.SecurityException(message: "Access Denied!");
        }
    }

    private static string[] ToPrivilegeSet(string privileges) =>
        string.IsNullOrWhiteSpace(value: privileges)
            ? []
            : [.. privileges
                .Split(separator: ',', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(selector: privilege => privilege.ToLowerInvariant())
                .Distinct()];

    static UserRole ToLocalUserRole(DataUserRole item) =>
        new()
        {
            RoleId = item.RoleId,
            UserId = item.UserId,
            User = item.User == null ? null : new User
            {
                Id = item.User.Id,
                DefaultCultureId = item.User.DefaultCultureId,
                DisplayName = item.User.DisplayName,
                Email = item.User.Email,
                IsActive = item.User.IsActive,
                DefaultCulture = item.User.DefaultCulture,
            },
            Role = item.Role == null ? null : new Role
            {
                Id = item.Role.Id,
                AppId = item.Role.AppId,
                Name = item.Role.Name,
                Description = item.Role.Description,
                Privs = item.Role.Privs,
            },
        };

    static DataUserRole ToExternalUserRole(UserRole item) =>
        new()
        {
            RoleId = item.RoleId,
            UserId = item.UserId,
            User = item.User == null ? null : new cCoder.Data.Models.Security.User
            {
                Id = item.User.Id,
                DefaultCultureId = item.User.DefaultCultureId,
                DisplayName = item.User.DisplayName,
                Email = item.User.Email,
                IsActive = item.User.IsActive,
                DefaultCulture = item.User.DefaultCulture as cCoder.Data.Models.CMS.Culture,
            },
            Role = item.Role == null ? null : new cCoder.Data.Models.Security.Role
            {
                Id = item.Role.Id,
                AppId = item.Role.AppId,
                Name = item.Role.Name,
                Description = item.Role.Description,
                Privs = item.Role.Privs,
            },
        };
}