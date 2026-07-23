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

            authorizationBroker.Authorize(appId: userBroker.GetAppId(entity: internalUser), privilege: $"{nameof(User)}_create");
            DataUser result = await userBroker.AddUserAsync(entity: internalUser);
            newUser.Id = result.Id;
            newUser.DefaultCultureId = result.DefaultCultureId;
            newUser.DisplayName = result.DisplayName;
            newUser.Email = result.Email;
            newUser.IsActive = result.IsActive;
            return newUser;

        });

    public ValueTask<User> UpdateUserAsync(User updatedUser) =>
        TryCatch(operation: async ValueTask<User> () =>
        {
            ValidateUserOnUpdate(
                updatedUser: updatedUser);

            DataUser internalUser = new()
            {
                Id = updatedUser.Id,
                DefaultCultureId = updatedUser.DefaultCultureId,
                DisplayName = updatedUser.DisplayName,
                Email = updatedUser.Email,
                IsActive = updatedUser.IsActive
            };

            authorizationBroker.Authorize(appId: userBroker.GetAppId(entity: internalUser), privilege: $"{nameof(User)}_update");
            DataUser result = await userBroker.UpdateUserAsync(entity: internalUser);
            updatedUser.Id = result.Id;
            updatedUser.DefaultCultureId = result.DefaultCultureId;
            updatedUser.DisplayName = result.DisplayName;
            updatedUser.Email = result.Email;
            updatedUser.IsActive = result.IsActive;
            return updatedUser;

        });

    public ValueTask DeleteAsync(string userId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateUserOnDelete(
                userId: userId);

            User user = GetValue(userId: userId);
            DataUser internalUser = ToExternalUser(item: user);
            authorizationBroker.Authorize(appId: userBroker.GetAppId(entity: internalUser), privilege: $"{nameof(User)}_delete");
            _ = await userBroker.DeleteUserAsync(entity: internalUser);

        });

    static User ToLocalUser(DataUser item) =>
        new()
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

    static DataUser ToExternalUser(User item) =>
        new()
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