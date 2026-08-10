// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class PageRoleOrchestrationService(
    IRoleProcessingService roleProcessingService,
    IPageRoleProcessingService pageRoleProcessingService)
        : IPageRoleOrchestrationService
{
    public ValueTask AddOrUpdateAppPageRolesAsync(App app) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidatePageRolesOnAddOrUpdate(app: app);
            Role[] requestedRoles = app?.Roles?.ToArray() ?? [];

            if (requestedRoles.Length == 0)
            {
                return;
            }

            Dictionary<string, Role> persistedRoles = roleProcessingService
                .GetAll(ignoreFilters: true)
                .Where(predicate: role => role.AppId == app.Id)
                .ToArray()
                .Where(predicate: role => !string.IsNullOrWhiteSpace(value: role.Name))
                .GroupBy(
                    keySelector: role => role.Name,
                    comparer: StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    keySelector: group => group.Key,
                    elementSelector: group => group.First(),
                    comparer: StringComparer.OrdinalIgnoreCase);

            foreach (Role requestedRole in requestedRoles)
            {
                if (persistedRoles.TryGetValue(
                    key: requestedRole.Name,
                    value: out Role persistedRole))
                {
                    requestedRole.Id = persistedRole.Id;
                    requestedRole.AppId = persistedRole.AppId;
                }
            }

            await pageRoleProcessingService.AddOrUpdatePageRolesAsync(
                roles: requestedRoles);
        });
}