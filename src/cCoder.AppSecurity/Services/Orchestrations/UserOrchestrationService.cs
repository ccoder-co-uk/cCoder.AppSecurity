// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class UserOrchestrationService(
    IUserProcessingService processingService,
    IUserEventProcessingService eventService
) : IUserOrchestrationService
{
    public User Get(string userId) =>
        TryCatch(operation: User () =>
        {
            ValidateGet(
                userId: userId);

            return processingService.Get(id: userId);
        });

    public User GetByEmail(string email, bool ignoreFilters = false) =>
        TryCatch(operation: User () =>
        {
            ValidateGetByEmail(
                email: email,
                ignoreFilters: ignoreFilters);

            return processingService.GetByEmail(email: email, ignoreFilters: ignoreFilters);
        });

    public IQueryable<User> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<User> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<User> AddUserAsync(User newUser) =>
        TryCatch(operation: async ValueTask<User> () =>
        {
            ValidateAddUser(
                newUser: newUser);

            var result = await processingService.AddUserAsync(entity: newUser);
            await eventService.RaiseUserAddEventAsync(entity: result);
            return result;

        });

    public ValueTask<User> UpdateUserAsync(User updatedUser) =>
        TryCatch(operation: async ValueTask<User> () =>
        {
            ValidateUpdateUser(
                updatedUser: updatedUser);

            var result = await processingService.UpdateUserAsync(entity: updatedUser);
            await eventService.RaiseUserUpdateEventAsync(entity: result);
            return result;

        });

    public ValueTask DeleteAsync(string userId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDelete(
                userId: userId);

            var entity = processingService.Get(id: userId);
            await eventService.RaiseUserDeleteEventAsync(entity: entity);
            await processingService.DeleteAsync(id: userId);

        });

    public ValueTask<IEnumerable<Result<User>>> AddOrUpdateUser(
        IEnumerable<User> items
    ) =>
        TryCatch(operation: ValueTask<IEnumerable<Result<User>>> () =>
        {
            ValidateAddOrUpdateUser(
                items: items);

            return processingService.AddOrUpdateUser(items: items);
        });

    public ValueTask DeleteAllUserAsync(IEnumerable<User> deletedUser) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateDeleteAllUser(
                deletedUser: deletedUser);

            return processingService.DeleteAllUserAsync(items: deletedUser);
        });
}