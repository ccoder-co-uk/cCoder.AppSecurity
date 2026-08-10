// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Aggregations;

internal sealed partial class AppRelationshipAggregationService(
    IAppOrchestrationService appOrchestrationService,
    IPageRoleOrchestrationService pageRoleOrchestrationService)
        : IAppRelationshipAggregationService
{
    public ValueTask AddAppAsync(App newApp) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateAppOnAdd(app: newApp);
            await appOrchestrationService.AddAppAsync(app: newApp);
            await pageRoleOrchestrationService.AddOrUpdateAppPageRolesAsync(app: newApp);
        });

    public ValueTask UpdateAppAsync(App updatedApp) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateAppOnUpdate(app: updatedApp);
            await appOrchestrationService.UpdateAppAsync(app: updatedApp);
            await pageRoleOrchestrationService.AddOrUpdateAppPageRolesAsync(app: updatedApp);
        });

    public ValueTask DeleteAppAsync(App deletedApp) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateAppOnDelete(app: deletedApp);
            await appOrchestrationService.DeleteAsync(appId: deletedApp.Id);
        });
}