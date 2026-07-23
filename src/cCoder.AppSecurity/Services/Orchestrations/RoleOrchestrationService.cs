// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class RoleOrchestrationService(
    IRoleProcessingService processingService,
    IRoleEventProcessingService eventService
) : IRoleOrchestrationService
{
    public Role Get(Guid roleId) =>
        TryCatch(operation: Role () =>
        {
            ValidateGet(
                roleId: roleId);

            return processingService.Get(id: roleId);
        });

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<Role> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<Role> AddRoleAsync(Role newRole) =>
        TryCatch(operation: async ValueTask<Role> () =>
        {
            ValidateAddRole(
                newRole: newRole);

            var result = await processingService.AddRoleAsync(entity: newRole);
            await eventService.RaiseRoleAddEventAsync(entity: result);
            return result;

        });

    public ValueTask<Role> AddValidatedRoleAsync(Role newRole) =>
        TryCatch(operation: async ValueTask<Role> () =>
        {
            ValidateAddValidatedRole(
                newRole: newRole);

            var result = await processingService.AddValidatedRoleAsync(entity: newRole);
            await eventService.RaiseRoleAddEventAsync(entity: result);
            return result;

        });

    public ValueTask<Role> UpdateRoleAsync(Role updatedRole) =>
        TryCatch(operation: async ValueTask<Role> () =>
        {
            ValidateUpdateRole(
                updatedRole: updatedRole);

            var result = await processingService.UpdateRoleAsync(entity: updatedRole);
            await eventService.RaiseRoleUpdateEventAsync(entity: result);
            return result;

        });

    public ValueTask<Role> UpdateValidatedRoleAsync(Role updatedRole) =>
        TryCatch(operation: async ValueTask<Role> () =>
        {
            ValidateUpdateValidatedRole(
                updatedRole: updatedRole);

            var result = await processingService.UpdateValidatedRoleAsync(entity: updatedRole);
            await eventService.RaiseRoleUpdateEventAsync(entity: result);
            return result;

        });

    public ValueTask DeleteAsync(Guid roleId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDelete(
                roleId: roleId);

            var entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: item => item.Id == roleId);

            if (entity is null)
            {
                return;
            }

            await eventService.RaiseRoleDeleteEventAsync(entity: entity);
            await processingService.DeleteAsync(id: roleId);

        });

    public ValueTask DeleteValidatedAsync(Guid roleId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDeleteValidated(
                roleId: roleId);

            var entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: item => item.Id == roleId);

            if (entity is null)
            {
                return;
            }

            await eventService.RaiseRoleDeleteEventAsync(entity: entity);
            await processingService.DeleteValidatedAsync(id: roleId);

        });

    public ValueTask<IEnumerable<Result<Role>>> AddOrUpdateRole(
        IEnumerable<Role> items
    ) =>
        TryCatch(operation: ValueTask<IEnumerable<Result<Role>>> () =>
        {
            ValidateAddOrUpdateRole(
                items: items);

            return processingService.AddOrUpdateRole(items: items);
        });

    public ValueTask ImportRoleAsync(int appId, IEnumerable<Role> roles) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateImportRole(
                appId: appId,
                roles: roles);

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

        });

    public ValueTask DeleteAllRoleAsync(IEnumerable<Role> deletedRole) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateDeleteAllRole(
                deletedRole: deletedRole);

            return processingService.DeleteAllRoleAsync(items: deletedRole);
        });
}