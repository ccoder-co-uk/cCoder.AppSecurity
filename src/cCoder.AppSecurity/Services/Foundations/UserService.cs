using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IUserBroker = cCoder.AppSecurity.Brokers.Storages.IUserBroker;


namespace cCoder.AppSecurity.Services.Foundations;

internal class UserService(IUserBroker userBroker, IAuthorizationBroker authorizationBroker)
    : IUserService
{
    public User Get(string id)
    {
        User user = GetAll().FirstOrDefault(i => i.Id == id);
        if (user is not null)
            return user;

        User unrestrictedUser = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedUser is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public User GetByEmail(string email, bool ignoreFilters = false) =>
        ToLocalUser(userBroker.GetUserByEmail(email, ignoreFilters));

    public IQueryable<User> GetAll(bool ignoreFilters = false) =>
        userBroker.GetAllUsers(ignoreFilters);

    public async ValueTask<User> AddAsync(User user)
    {
        DataUser internalUser = new()
        {
            Id = user.Id,
            DefaultCultureId = user.DefaultCultureId,
            DisplayName = user.DisplayName,
            Email = user.Email,
            IsActive = user.IsActive
        };
        authorizationBroker.Authorize(userBroker.GetAppId(internalUser), $"{nameof(User)}_create");
        DataUser result = await userBroker.AddUserAsync(internalUser);
        user.Id = result.Id;
        user.DefaultCultureId = result.DefaultCultureId;
        user.DisplayName = result.DisplayName;
        user.Email = result.Email;
        user.IsActive = result.IsActive;
        return user;
    }

    public async ValueTask<User> UpdateAsync(User user)
    {
        DataUser internalUser = new()
        {
            Id = user.Id,
            DefaultCultureId = user.DefaultCultureId,
            DisplayName = user.DisplayName,
            Email = user.Email,
            IsActive = user.IsActive
        };
        authorizationBroker.Authorize(userBroker.GetAppId(internalUser), $"{nameof(User)}_update");
        DataUser result = await userBroker.UpdateUserAsync(internalUser);
        user.Id = result.Id;
        user.DefaultCultureId = result.DefaultCultureId;
        user.DisplayName = result.DisplayName;
        user.Email = result.Email;
        user.IsActive = result.IsActive;
        return user;
    }

    public async ValueTask DeleteAsync(string id)
    {
        User user = Get(id);
        DataUser internalUser = ToExternalUser(user);
        authorizationBroker.Authorize(userBroker.GetAppId(internalUser), $"{nameof(User)}_delete");
        _ = await userBroker.DeleteUserAsync(internalUser);
    }

    static User ToLocalUser(DataUser item) =>
        new()
        {
            Id = item.Id,
            DefaultCultureId = item.DefaultCultureId,
            DisplayName = item.DisplayName,
            Email = item.Email,
            IsActive = item.IsActive,
            DefaultCulture = item.DefaultCulture,
            Roles = item.Roles?.Select(ToLocalUserRole).ToArray(),
        };

    static DataUser ToExternalUser(User item) =>
        new()
        {
            Id = item.Id,
            DefaultCultureId = item.DefaultCultureId,
            DisplayName = item.DisplayName,
            Email = item.Email,
            IsActive = item.IsActive,
            DefaultCulture = item.DefaultCulture as cCoder.Data.Models.CMS.Culture,
            Roles = item.Roles?.Select(ToExternalUserRole).ToArray(),
        };

    static UserRole ToLocalUserRole(DataUserRole item) =>
        new()
        {
            RoleId = item.RoleId,
            UserId = item.UserId,
            User = null,
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
            User = null,
            Role = item.Role == null ? null : new DataRole
            {
                Id = item.Role.Id,
                AppId = item.Role.AppId,
                Name = item.Role.Name,
                Description = item.Role.Description,
                Privs = item.Role.Privs,
            },
        };
}













