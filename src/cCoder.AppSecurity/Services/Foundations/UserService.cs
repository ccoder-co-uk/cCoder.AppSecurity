// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

internal sealed partial class UserService(IUserBroker userBroker, IAuthorizationBroker authorizationBroker)
    : IUserService
{
    public User Get(string userId) =>
        TryCatch(operation: User () =>
        {
            ValidateUserOnGet(
                userId: userId);

            User user = GetAllValue()
                .FirstOrDefault(predicate: i => i.Id == userId);

            if (user is not null)
            {
                return user;
            }

            User unrestrictedUser = GetAllValue(ignoreFilters: true)
                .FirstOrDefault(predicate: i => i.Id == userId);

            if (unrestrictedUser is not null)
            {
                throw new SecurityException(message: "Access Denied!");
            }

            return null;

        });

    public User GetByEmail(string email, bool ignoreFilters = false) =>
        TryCatch(operation: User () =>
        {
            ValidateByEmailOnGet(
                email: email,
                ignoreFilters: ignoreFilters);

            return ToLocalUser(item: userBroker.GetUserByEmail(email: email, ignoreFilters: ignoreFilters));
        });

    public IQueryable<User> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<User> () =>
        {
            ValidateAllOnGet(
                ignoreFilters: ignoreFilters);

            return userBroker.GetAllUsers(ignoreFilters: ignoreFilters);
        });

    public ValueTask<User> AddUserAsync(User newUser) =>
        TryCatch(operation: async ValueTask<User> () =>
        {
            ValidateUserOnAdd(
                newUser: newUser);

            DataUser internalUser = new()
            {
                Id = newUser.Id,
                DefaultCultureId = newUser.DefaultCultureId,
                DisplayName = newUser.DisplayName,
                Email = newUser.Email,
                IsActive = newUser.IsActive
            };

            bool isFirstUser = !userBroker
                .GetAllUsers(ignoreFilters: true)
                .Any(predicate: user =>
                    user.Id != "Guest"
                    && user.Id != "system");

            if (!isFirstUser)
            {
                authorizationBroker.Authorize(
                    appId: userBroker.GetAppId(entity: internalUser),
                    privilege: $"{nameof(User)}_create");
            }

            DataUser result = await userBroker.AddUserAsync(entity: internalUser);
            return MapAddedUser(newUser: newUser, result: result);

        });

    public ValueTask<User> AddUserFromAccountEventAsync(User newUser) =>
        TryCatch(operation: async ValueTask<User> () =>
        {
            ValidateUserFromAccountEventOnAdd(
                newUser: newUser);

            DataUser result = await userBroker.AddUserAsync(
                entity: ToExternalUser(item: newUser));

            return MapAddedUser(newUser: newUser, result: result);

        });

    public ValueTask<User> UpdateUserAsync(User updatedUser) =>
        TryCatch(operation: async ValueTask<User> () =>
        {
            ValidateUserOnUpdate(
                updatedUser: updatedUser);

            DataUser internalUser = ToExternalUser(item: updatedUser);

            authorizationBroker.Authorize(
                appId: userBroker.GetAppId(entity: internalUser),
                privilege: $"{nameof(User)}_update");

            return await UpdateUserValueAsync(
                updatedUser: updatedUser,
                internalUser: internalUser);

        });

    public ValueTask<User> UpdateUserFromAccountEventAsync(
        User updatedUser) =>
        TryCatch(operation: async ValueTask<User> () =>
        {
            ValidateUserFromAccountEventOnUpdate(
                updatedUser: updatedUser);

            return await UpdateUserValueAsync(
                updatedUser: updatedUser,
                internalUser: ToExternalUser(item: updatedUser));

        });

    public ValueTask DeleteAsync(string userId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateUserOnDelete(
                userId: userId);

            User user = GetValue(userId: userId);

            if (user is null)
            {
                return;
            }

            DataUser internalUser = ToExternalUser(item: user);
            authorizationBroker.Authorize(appId: userBroker.GetAppId(entity: internalUser), privilege: $"{nameof(User)}_delete");
            _ = await userBroker.DeleteUserAsync(entity: internalUser);

        });

    static User ToLocalUser(DataUser item) =>
        item is null
            ? null
            : new User
        {
            Id = item.Id,
            DefaultCultureId = item.DefaultCultureId,
            DisplayName = item.DisplayName,
            Email = item.Email,
            IsActive = item.IsActive,
            DefaultCulture = item.DefaultCulture,
            Roles = item.Roles?.Select(selector: ToLocalUserRole)
                .ToArray(),
            };

    private async ValueTask<User> UpdateUserValueAsync(
        User updatedUser,
        DataUser internalUser)
    {
        DataUser result = await userBroker.UpdateUserAsync(
            entity: internalUser);

        updatedUser.Id = result.Id;
        updatedUser.DefaultCultureId = result.DefaultCultureId;
        updatedUser.DisplayName = result.DisplayName;
        updatedUser.Email = result.Email;
        updatedUser.IsActive = result.IsActive;
        return updatedUser;
    }

    static DataUser ToExternalUser(User item) =>
        item is null
            ? null
            : new DataUser
        {
            Id = item.Id,
            DefaultCultureId = item.DefaultCultureId,
            DisplayName = item.DisplayName,
            Email = item.Email,
            IsActive = item.IsActive,
            DefaultCulture = item.DefaultCulture as cCoder.Data.Models.CMS.Culture,
            Roles = item.Roles?.Select(selector: ToExternalUserRole)
                .ToArray(),
            };

    private static User MapAddedUser(User newUser, DataUser result)
    {
        newUser.Id = result.Id;
        newUser.DefaultCultureId = result.DefaultCultureId;
        newUser.DisplayName = result.DisplayName;
        newUser.Email = result.Email;
        newUser.IsActive = result.IsActive;
        return newUser;
    }

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

    private User GetValue(string userId) =>
        Get(userId: userId);

    private IQueryable<User> GetAllValue(bool ignoreFilters = false) =>
        GetAll(ignoreFilters: ignoreFilters);
}