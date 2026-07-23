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

    public async ValueTask<Role> AddRoleAsync(Role entity)
    {
        var result = await processingService.AddRoleAsync(entity: entity);
        await eventService.RaiseRoleAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> AddValidatedRoleAsync(Role entity)
    {
        var result = await processingService.AddValidatedRoleAsync(entity: entity);
        await eventService.RaiseRoleAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> UpdateRoleAsync(Role entity)
    {
        var result = await processingService.UpdateRoleAsync(entity: entity);
        await eventService.RaiseRoleUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> UpdateValidatedRoleAsync(Role entity)
    {
        var result = await processingService.UpdateValidatedRoleAsync(entity: entity);
        await eventService.RaiseRoleUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        var entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseRoleDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask DeleteValidatedAsync(Guid id)
    {
        var entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseRoleDeleteEventAsync(entity: entity);
        await processingService.DeleteValidatedAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<Role>>> AddOrUpdateRole(
        IEnumerable<Role> items
    ) =>
        processingService.AddOrUpdateRole(items: items);

    public async ValueTask ImportRoleAsync(int appId, IEnumerable<Role> roles)
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
                await processingService.AddValidatedRoleAsync(entity: role);
            }
            else
            {
                await processingService.UpdateValidatedRoleAsync(entity: role);
            }
        }
    }

    public ValueTask DeleteAllRoleAsync(IEnumerable<Role> items) =>
        processingService.DeleteAllRoleAsync(items: items);
}