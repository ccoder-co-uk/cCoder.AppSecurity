// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class PrivilegeOrchestrationService(
    IPrivilegeProcessingService processingService,
    IPrivilegeEventProcessingService eventService
) : IPrivilegeOrchestrationService
{
    public Privilege Get(string privilegeId) =>
        TryCatch(operation: Privilege () =>
        {
            ValidateGet(
                privilegeId: privilegeId);

            return processingService.Get(id: privilegeId);
        });

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<Privilege> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<Privilege> AddPrivilegeAsync(Privilege newPrivilege) =>
        TryCatch(operation: async ValueTask<Privilege> () =>
        {
            ValidateAddPrivilege(
                newPrivilege: newPrivilege);

            var result = await processingService.AddPrivilegeAsync(entity: newPrivilege);
            await eventService.RaisePrivilegeAddEventAsync(entity: result);
            return result;

        });

    public ValueTask<Privilege> UpdatePrivilegeAsync(Privilege updatedPrivilege) =>
        TryCatch(operation: async ValueTask<Privilege> () =>
        {
            ValidateUpdatePrivilege(
                updatedPrivilege: updatedPrivilege);

            var result = await processingService.UpdatePrivilegeAsync(entity: updatedPrivilege);
            await eventService.RaisePrivilegeUpdateEventAsync(entity: result);
            return result;

        });

    public ValueTask DeleteAsync(string privilegeId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDelete(
                privilegeId: privilegeId);

            var entity = processingService.Get(id: privilegeId);
            await eventService.RaisePrivilegeDeleteEventAsync(entity: entity);
            await processingService.DeleteAsync(id: privilegeId);

        });

    public ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdatePrivilege(
        IEnumerable<Privilege> items
    ) =>
        TryCatch(operation: ValueTask<IEnumerable<Result<Privilege>>> () =>
        {
            ValidateAddOrUpdatePrivilege(
                items: items);

            return processingService.AddOrUpdatePrivilege(items: items);
        });

    public ValueTask DeleteAllPrivilegeAsync(IEnumerable<Privilege> deletedPrivilege) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateDeleteAllPrivilege(
                deletedPrivilege: deletedPrivilege);

            return processingService.DeleteAllPrivilegeAsync(items: deletedPrivilege);
        });
}