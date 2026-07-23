// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal class PrivilegeOrchestrationService(
    IPrivilegeProcessingService processingService,
    IPrivilegeEventProcessingService eventService
) : IPrivilegeOrchestrationService
{
    public Privilege Get(string id) =>
        processingService.Get(id: id);

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Privilege> AddAsync(Privilege entity)
    {
        var result = await processingService.AddAsync(entity: entity);
        await eventService.RaisePrivilegeAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Privilege> UpdateAsync(Privilege entity)
    {
        var result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaisePrivilegeUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(string id)
    {
        var entity = processingService.Get(id: id);
        await eventService.RaisePrivilegeDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdate(
        IEnumerable<Privilege> items
    ) =>
        processingService.AddOrUpdate(items: items);

    public ValueTask DeleteAllAsync(IEnumerable<Privilege> items) =>
        processingService.DeleteAllAsync(items: items);
}