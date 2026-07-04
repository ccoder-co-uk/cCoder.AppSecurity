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
        userRoleBroker.GetAllUserRoles(ignoreFilters);

    public async ValueTask<UserRole> AddAsync(UserRole userRole)
    {
        DataUserRole internalUserRole = new()
        {
            RoleId = userRole.RoleId,
            UserId = userRole.UserId
        };
        int? appId = userRoleBroker.GetAppId(internalUserRole);
        authorizationBroker.Authorize(appId, $"{nameof(UserRole)}_create");
        AuthorizeAssignedRolePrivileges(appId, userRole.RoleId);
        DataUserRole result = await userRoleBroker.AddUserRoleAsync(internalUserRole);
        userRole.RoleId = result.RoleId;
        userRole.UserId = result.UserId;
        return userRole;
    }

    public async ValueTask<UserRole> AddValidatedAsync(UserRole userRole)
    {
        DataUserRole result = await userRoleBroker.AddUserRoleAsync(ToExternalUserRole(userRole));
        userRole.RoleId = result.RoleId;
        userRole.UserId = result.UserId;

        return userRole;
    }

    public async ValueTask DeleteAsync(UserRole userRole)
    {
        DataUserRole internalUserRole = ToExternalUserRole(userRole);
        authorizationBroker.Authorize(
            userRoleBroker.GetAppId(internalUserRole),
            $"{nameof(UserRole)}_delete"
        );
        _ = await userRoleBroker.DeleteUserRoleAsync(internalUserRole);
    }

    private void AuthorizeAssignedRolePrivileges(int? appId, Guid roleId)
    {
        if (!appId.HasValue)
            return;

        Role role = roleBroker.GetAllRoles(true).FirstOrDefault(foundRole => foundRole.Id == roleId);

        if (role is null)
            return;

        string[] assignedPrivilegeSet = ToPrivilegeSet(role.Privs);

        if (assignedPrivilegeSet.Length == 0)
            return;

        User currentUser = authorizationBroker.GetCurrentUser();
        HashSet<string> grantedPrivileges = currentUser?.Roles?
            .Where(userRole => userRole.Role?.AppId == appId.Value)
            .SelectMany(userRole => ToPrivilegeSet(userRole.Role.Privs))
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? [];

        if (assignedPrivilegeSet.Any(assignedPrivilege => !grantedPrivileges.Contains(assignedPrivilege)))
            throw new System.Security.SecurityException("Access Denied!");
    }

    private static string[] ToPrivilegeSet(string privileges) =>
        string.IsNullOrWhiteSpace(privileges)
            ? []
            : [.. privileges
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(privilege => privilege.ToLowerInvariant())
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













