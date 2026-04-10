using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IUserRoleBroker = cCoder.AppSecurity.Brokers.Storages.IUserRoleBroker;


namespace cCoder.AppSecurity.Services.Foundations;

internal class UserRoleService(
    IUserRoleBroker userRoleBroker,
    IAuthorizationBroker authorizationBroker
) : IUserRoleService
{
    public IQueryable<UserRole> GetAll(bool ignoreFilters = false) =>
        userRoleBroker.GetAllUserRoles(ignoreFilters);

    public async ValueTask<UserRole> AddAsync(UserRole userRole)
    {
        DataUserRole internalUserRole = ToExternalUserRole(userRole);
        authorizationBroker.Authorize(
            userRoleBroker.GetAppId(internalUserRole),
            $"{nameof(UserRole)}_create"
        );
        return ToLocalUserRole(await userRoleBroker.AddUserRoleAsync(internalUserRole));
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













