// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal class UserRoleOrchestrationService(
    IUserRoleProcessingService processingService,
    IUserRoleEventProcessingService eventService
) : IUserRoleOrchestrationService
{
    public IQueryable<UserRole> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<UserRole> AddAsync(UserRole entity)
    {
        var result = await processingService.AddAsync(entity);
        await eventService.RaiseUserRoleAddEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(UserRole entity)
    {
        await eventService.RaiseUserRoleDeleteEventAsync(entity);
        await processingService.DeleteAsync(entity);
    }

    public ValueTask<IEnumerable<Result<UserRole>>> AddOrUpdate(
        IEnumerable<UserRole> items
    ) => processingService.AddOrUpdate(items);

    public ValueTask DeleteAllAsync(IEnumerable<UserRole> items) =>
        processingService.DeleteAllAsync(items);

    public ValueTask<UserRole> SaveAsync(UserRole entity) => processingService.SaveAsync(entity);
}