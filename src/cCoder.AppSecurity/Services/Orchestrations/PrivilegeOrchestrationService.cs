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
    public Privilege Get(string privilegeId) =>
        processingService.Get(id: privilegeId);

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<Privilege> AddPrivilegeAsync(Privilege newPrivilege)
    {
        var result = await processingService.AddPrivilegeAsync(entity: newPrivilege);
        await eventService.RaisePrivilegeAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Privilege> UpdatePrivilegeAsync(Privilege updatedPrivilege)
    {
        var result = await processingService.UpdatePrivilegeAsync(entity: updatedPrivilege);
        await eventService.RaisePrivilegeUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(string privilegeId)
    {
        var entity = processingService.Get(id: privilegeId);
        await eventService.RaisePrivilegeDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: privilegeId);
    }

    public ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdatePrivilege(
        IEnumerable<Privilege> items
    ) =>
        processingService.AddOrUpdatePrivilege(items: items);

    public ValueTask DeleteAllPrivilegeAsync(IEnumerable<Privilege> deletedPrivilege) =>
        processingService.DeleteAllPrivilegeAsync(items: deletedPrivilege);
}