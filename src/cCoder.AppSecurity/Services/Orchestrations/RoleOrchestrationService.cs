// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal class RoleOrchestrationService(
    IRoleProcessingService processingService,
    IRoleEventProcessingService eventService
) : IRoleOrchestrationService
{
    public Role Get(Guid id) =>
        processingService.Get(id: id);

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Role> AddAsync(Role entity)
    {
        var result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseRoleAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> AddValidatedAsync(Role entity)
    {
        var result = await processingService.AddValidatedAsync(entity: entity);
        await eventService.RaiseRoleAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> UpdateAsync(Role entity)
    {
        var result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseRoleUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> UpdateValidatedAsync(Role entity)
    {
        var result = await processingService.UpdateValidatedAsync(entity: entity);
        await eventService.RaiseRoleUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        var entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseRoleDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask DeleteValidatedAsync(Guid id)
    {
        var entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseRoleDeleteEventAsync(entity: entity);
        await processingService.DeleteValidatedAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<Role>>> AddOrUpdate(
        IEnumerable<Role> items
    ) =>
        processingService.AddOrUpdate(items: items);

    public async ValueTask ImportAsync(int appId, IEnumerable<Role> roles)
    {
        var dbVersions = processingService
            .GetAll(ignoreFilters: false)
            .Where(predicate: role => role.AppId == appId)
            .Select(selector: role => new { role.Id, role.Name })
            .ToArray();

        foreach (Role role in roles)
        {
            role.AppId = appId;
            role.Id = dbVersions.FirstOrDefault(predicate: existing => existing.Name == role.Name)?.Id ?? Guid.Empty;

            if (role.Id == Guid.Empty)
            {
                await processingService.AddValidatedAsync(entity: role);
            }
            else
            {
                await processingService.UpdateValidatedAsync(entity: role);
            }
        }
    }

    public ValueTask DeleteAllAsync(IEnumerable<Role> items) =>
        processingService.DeleteAllAsync(items: items);
}