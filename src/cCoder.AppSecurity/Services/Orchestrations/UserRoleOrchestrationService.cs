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
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<UserRole> AddUserRoleAsync(UserRole entity)
    {
        var result = await processingService.AddUserRoleAsync(entity: entity);
        await eventService.RaiseUserRoleAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteUserRoleAsync(UserRole entity)
    {
        await eventService.RaiseUserRoleDeleteEventAsync(entity: entity);
        await processingService.DeleteUserRoleAsync(entity: entity);
    }

    public ValueTask<IEnumerable<Result<UserRole>>> AddOrUpdateUserRole(
        IEnumerable<UserRole> items
    ) =>
        processingService.AddOrUpdateUserRole(items: items);

    public ValueTask DeleteAllUserRoleAsync(IEnumerable<UserRole> items) =>
        processingService.DeleteAllUserRoleAsync(items: items);

    public ValueTask<UserRole> SaveUserRoleAsync(UserRole entity) =>
        processingService.SaveUserRoleAsync(entity: entity);
}