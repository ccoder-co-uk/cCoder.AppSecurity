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
    public Privilege Get(string id) => processingService.Get(id);

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters);

    public async ValueTask<Privilege> AddAsync(Privilege entity)
    {
        var result = await processingService.AddAsync(entity);
        await eventService.RaisePrivilegeAddEventAsync(result);
        return result;
    }

    public async ValueTask<Privilege> UpdateAsync(Privilege entity)
    {
        var result = await processingService.UpdateAsync(entity);
        await eventService.RaisePrivilegeUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(string id)
    {
        var entity = processingService.Get(id);
        await eventService.RaisePrivilegeDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdate(
        IEnumerable<Privilege> items
    ) => processingService.AddOrUpdate(items);

    public ValueTask DeleteAllAsync(IEnumerable<Privilege> items) =>
        processingService.DeleteAllAsync(items);
}









