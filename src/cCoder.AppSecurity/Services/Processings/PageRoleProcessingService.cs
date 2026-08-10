// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class PageRoleProcessingService(IPageRoleService pageRoleService)
    : IPageRoleProcessingService
{
    public ValueTask AddOrUpdatePageRolesAsync(IEnumerable<Role> roles) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidatePageRolesOnAddOrUpdate(roles: roles);
            Role[] requestedRoles = roles?.ToArray() ?? [];

            if (requestedRoles.Length == 0)
            {
                return;
            }

            PageRole[] existingPageRoles = pageRoleService.GetAll()
                .ToArray();

            foreach (Role role in requestedRoles)
            {
                foreach (PageRole requestedPageRole in role.Pages ?? [])
                {
                    int pageId = requestedPageRole.PageId == 0
                        ? pageRoleService.GetPageId(
                            appId: role.AppId,
                            path: requestedPageRole.Page?.Path ?? string.Empty)
                        : requestedPageRole.PageId;

                    ValidatePageRoleDependenciesOnAddOrUpdate(
                        roleId: role.Id,
                        pageId: pageId);

                    requestedPageRole.RoleId = role.Id;
                    requestedPageRole.PageId = pageId;

                    bool exists = existingPageRoles.Any(predicate: pageRole =>
                        pageRole.RoleId == role.Id
                        && pageRole.PageId == pageId);

                    if (!exists)
                    {
                        _ = await pageRoleService.AddPageRoleAsync(newPageRole: new PageRole
                        {
                            RoleId = role.Id,
                            PageId = pageId,
                        });
                    }
                }
            }
        });
}