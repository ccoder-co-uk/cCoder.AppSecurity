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
    public User Get(string id) => processingService.Get(id);

    public User GetByEmail(string email, bool ignoreFilters = false) =>
        processingService.GetByEmail(email, ignoreFilters);

    public IQueryable<User> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<User> AddAsync(User entity)
    {
        var result = await processingService.AddAsync(entity);
        await eventService.RaiseUserAddEventAsync(result);
        return result;
    }

    public async ValueTask<User> UpdateAsync(User entity)
    {
        var result = await processingService.UpdateAsync(entity);
        await eventService.RaiseUserUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(string id)
    {
        var entity = processingService.Get(id);
        await eventService.RaiseUserDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<User>>> AddOrUpdate(
        IEnumerable<User> items
    ) => processingService.AddOrUpdate(items);

    public ValueTask DeleteAllAsync(IEnumerable<User> items) =>
        processingService.DeleteAllAsync(items);
}