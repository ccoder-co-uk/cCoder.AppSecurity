// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Processings;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class UserRoleOrchestrationService(
    IUserRoleProcessingService processingService,
    IUserRoleEventProcessingService eventService
) : IUserRoleOrchestrationService
{
    public IQueryable<UserRole> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<UserRole> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole) =>
        TryCatch(operation: async ValueTask<UserRole> () =>
        {
            ValidateAddUserRole(
                newUserRole: newUserRole);

            var result = await processingService.AddUserRoleAsync(userRole: newUserRole);
            await eventService.RaiseUserRoleAddEventAsync(userRole: result);
            return result;

        });

    public ValueTask DeleteUserRoleAsync(UserRole deletedUserRole) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDeleteUserRole(
                deletedUserRole: deletedUserRole);

            await eventService.RaiseUserRoleDeleteEventAsync(userRole: deletedUserRole);
            await processingService.DeleteUserRoleAsync(userRole: deletedUserRole);

        });

    public ValueTask DeleteAllUserRoleAsync(IEnumerable<UserRole> deletedUserRole) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateDeleteAllUserRole(
                deletedUserRole: deletedUserRole);

            return processingService.DeleteAllUserRoleAsync(items: deletedUserRole);
        });

    public ValueTask<UserRole> SaveUserRoleAsync(UserRole userRole) =>
        TryCatch(operation: ValueTask<UserRole> () =>
        {
            ValidateSaveUserRole(
                userRole: userRole);

            return processingService.SaveUserRoleAsync(userRole: userRole);
        });
}