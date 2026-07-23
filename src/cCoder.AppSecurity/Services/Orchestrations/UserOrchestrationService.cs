// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal class UserOrchestrationService(
    IUserProcessingService processingService,
    IUserEventProcessingService eventService
) : IUserOrchestrationService
{
    public User Get(string userId) =>
        processingService.Get(id: userId);

    public User GetByEmail(string email, bool ignoreFilters = false) =>
        processingService.GetByEmail(email: email, ignoreFilters: ignoreFilters);

    public IQueryable<User> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<User> AddUserAsync(User newUser)
    {
        var result = await processingService.AddUserAsync(entity: newUser);
        await eventService.RaiseUserAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<User> UpdateUserAsync(User updatedUser)
    {
        var result = await processingService.UpdateUserAsync(entity: updatedUser);
        await eventService.RaiseUserUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(string userId)
    {
        var entity = processingService.Get(id: userId);
        await eventService.RaiseUserDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: userId);
    }

    public ValueTask<IEnumerable<Result<User>>> AddOrUpdateUser(
        IEnumerable<User> items
    ) =>
        processingService.AddOrUpdateUser(items: items);

    public ValueTask DeleteAllUserAsync(IEnumerable<User> deletedUser) =>
        processingService.DeleteAllUserAsync(items: deletedUser);
}