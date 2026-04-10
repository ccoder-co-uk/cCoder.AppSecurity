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
    public Role Get(Guid id) => processingService.Get(id);

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Role> AddAsync(Role entity)
    {
        var result = await processingService.AddAsync(entity);
        await eventService.RaiseRoleAddEventAsync(result);
        return result;
    }

    public async ValueTask<Role> AddValidatedAsync(Role entity)
    {
        var result = await processingService.AddValidatedAsync(entity);
        await eventService.RaiseRoleAddEventAsync(result);
        return result;
    }

    public async ValueTask<Role> UpdateAsync(Role entity)
    {
        var result = await processingService.UpdateAsync(entity);
        await eventService.RaiseRoleUpdateEventAsync(result);
        return result;
    }

    public async ValueTask<Role> UpdateValidatedAsync(Role entity)
    {
        var result = await processingService.UpdateValidatedAsync(entity);
        await eventService.RaiseRoleUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        var entity = processingService.GetAll(true).FirstOrDefault(item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseRoleDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<Role>>> AddOrUpdate(
        IEnumerable<Role> items
    ) => processingService.AddOrUpdate(items);

    public ValueTask DeleteAllAsync(IEnumerable<Role> items) =>
        processingService.DeleteAllAsync(items);
}









