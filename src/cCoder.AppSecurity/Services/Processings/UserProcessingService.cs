// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class UserProcessingService(IUserService service, ICoreAuthInfo authInfo)
    : IUserProcessingService
{
    public User Get(string userId) =>
        TryCatch(operation: User () =>
        {
            ValidateGet(
                userId: userId);

            return service.Get(id: userId);
        });

    public User GetByEmail(string email, bool ignoreFilters = false) =>
        TryCatch(operation: User () =>
        {
            ValidateGetByEmail(
                email: email,
                ignoreFilters: ignoreFilters);

            return service.GetByEmail(email: email, ignoreFilters: ignoreFilters);
        });

    public IQueryable<User> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<User> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return service.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<User> AddUserAsync(User newUser) =>
        TryCatch(operation: async ValueTask<User> () =>
        {
            ValidateAddUser(
                newUser: newUser);

            User existingUser = service
                .GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: u => u.Id == newUser.Id || u.Email == newUser.Email);

            return existingUser != null ? existingUser : await service.AddUserAsync(user: newUser);

        });

    public ValueTask DeleteAsync(string userId) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateDelete(
                userId: userId);

            User dbVersion = Get(userId: userId);

            return authInfo.SSOUserId == dbVersion.Id
                ? service.DeleteAsync(id: userId)
                : throw new SecurityException(message: "Access Denied!");

        });

    public ValueTask<User> UpdateUserAsync(User updatedUser) =>
        TryCatch(operation: ValueTask<User> () =>
        {
            ValidateUpdateUser(
                updatedUser: updatedUser);

            return authInfo.SSOUserId == updatedUser.Id
            ? service.UpdateUserAsync(user: updatedUser)
            : throw new SecurityException(message: "Access Denied!");
        });

    public ValueTask<IEnumerable<Result<User>>> AddOrUpdateUser(
        IEnumerable<User> items
    ) =>
        TryCatch(operation: async ValueTask<IEnumerable<Result<User>>> () =>
        {
            ValidateAddOrUpdateUser(
                items: items);

            List<Result<User>> results = [];

            foreach (User item in items)
            {
                try
                {
                    bool isAdd = string.IsNullOrWhiteSpace(value: item.Id);

                    results.Add(
    item: new Result<User>
    {
        Success = true,
        Item = isAdd ? await AddUserAsync(newUser: item) : await UpdateUserAsync(updatedUser: item),
        Message = isAdd ? "Added Successfully" : "Updated Successfully",
    }
                    );
                }
                catch (Exception ex)
                {
                    results.Add(
    item: new Result<User>
    {
        Success = false,
        Item = item,
        Message = ex.Message,
    }
                    );
                }
            }

            return results;

        });

    public ValueTask DeleteAllUserAsync(IEnumerable<User> deletedUser) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDeleteAllUser(
                deletedUser: deletedUser);

            foreach (User item in deletedUser)
            {
                await DeleteAsync(userId: item.Id);
            }

        });
}