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
    public Role Get(Guid roleId) =>
        processingService.Get(id: roleId);

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Role> AddRoleAsync(Role newRole)
    {
        var result = await processingService.AddRoleAsync(entity: newRole);
        await eventService.RaiseRoleAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> AddValidatedRoleAsync(Role newRole)
    {
        var result = await processingService.AddValidatedRoleAsync(entity: newRole);
        await eventService.RaiseRoleAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> UpdateRoleAsync(Role updatedRole)
    {
        var result = await processingService.UpdateRoleAsync(entity: updatedRole);
        await eventService.RaiseRoleUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Role> UpdateValidatedRoleAsync(Role updatedRole)
    {
        var result = await processingService.UpdateValidatedRoleAsync(entity: updatedRole);
        await eventService.RaiseRoleUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid roleId)
    {
        var entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == roleId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseRoleDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: roleId);
    }

    public async ValueTask DeleteValidatedAsync(Guid roleId)
    {
        var entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == roleId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseRoleDeleteEventAsync(entity: entity);
        await processingService.DeleteValidatedAsync(id: roleId);
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

    public ValueTask DeleteAllRoleAsync(IEnumerable<Role> deletedRole) =>
        processingService.DeleteAllRoleAsync(items: deletedRole);
}