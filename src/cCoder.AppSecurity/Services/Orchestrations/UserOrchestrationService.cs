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
    public User Get(string id) =>
        processingService.Get(id: id);

    public User GetByEmail(string email, bool ignoreFilters = false) =>
        processingService.GetByEmail(email: email, ignoreFilters: ignoreFilters);

    public IQueryable<User> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<User> AddUserAsync(User entity)
    {
        var result = await processingService.AddUserAsync(entity: entity);
        await eventService.RaiseUserAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<User> UpdateUserAsync(User entity)
    {
        var result = await processingService.UpdateUserAsync(entity: entity);
        await eventService.RaiseUserUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(string id)
    {
        var entity = processingService.Get(id: id);
        await eventService.RaiseUserDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<User>>> AddOrUpdateUser(
        IEnumerable<User> items
    ) =>
        processingService.AddOrUpdateUser(items: items);

    public ValueTask DeleteAllUserAsync(IEnumerable<User> items) =>
        processingService.DeleteAllUserAsync(items: items);
}