// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AppSecurityPackageOrchestrationService(
    IRoleProcessingService roleProcessingService,
    IJsonProcessingService jsonProcessingService)
        : IAppSecurityPackageOrchestrationService
{
    public AppSecurityPackageMapping MapAppSecurityPackageMappingRoles(AppSecurityPackageMapping mapping) =>
        TryCatch(operation: AppSecurityPackageMapping () =>
        {
            ValidateAppSecurityPackageMappingRolesOnMap(mapping: mapping);
            int appId = mapping.AppId;
            AppSecurityPackage package = mapping.Package;
            Role[] roles = GetRoles(package: package);

            foreach (Role role in roles)
            {
                role.AppId = appId;
                role.Pages = [];
            }

            mapping.App = new App { Id = appId, Roles = roles };
            return mapping;
        });

    public AppSecurityPackageMapping MapAppSecurityPackageMappingPageRoles(
        AppSecurityPackageMapping mapping) =>
        TryCatch(operation: AppSecurityPackageMapping () =>
        {
            ValidateAppSecurityPackageMappingPageRolesOnMap(mapping: mapping);
            int appId = mapping.AppId;
            AppSecurityPackage package = mapping.Package;
            PageRoleInfo[] pageRoleInfos = GetPageRoleInfos(package: package);

            string[] roleNames = pageRoleInfos
                .Select(selector: pageRole => pageRole.Role)
                .Distinct(comparer: StringComparer.OrdinalIgnoreCase)
                .ToArray();

            Role[] roles = roleProcessingService.GetAll(ignoreFilters: true)
                .Where(predicate: role =>
                    role.AppId == appId
                    && roleNames.Contains(value: role.Name))
                .ToArray();

            AttachPageRoles(appId: appId, roles: roles, pageRoleInfos: pageRoleInfos);

            mapping.App = new App { Id = appId, Roles = roles };
            return mapping;
        });

    private Role[] GetRoles(AppSecurityPackage package) =>
        package.Items
            .Where(predicate: item => item.Type is "Core/Role" or "AppSecurity/Role")
            .SelectMany(selector: item => item.Data.StartsWith(value: "{")
                ? [jsonProcessingService.ParseJson<Role>(json: item.Data)]
                : jsonProcessingService.ParseJson<Role[]>(json: item.Data))
            .ToArray();

    private PageRoleInfo[] GetPageRoleInfos(AppSecurityPackage package) =>
        package.Items
            .Where(predicate: item => item.Type == "ContentManagement/PageRole")
            .SelectMany(selector: item => item.Data.StartsWith(value: "{")
                ? [jsonProcessingService.ParseJson<PageRoleInfo>(json: item.Data)]
                : jsonProcessingService.ParseJson<PageRoleInfo[]>(json: item.Data))
            .ToArray();

    private static void AttachPageRoles(
        int appId,
        IEnumerable<Role> roles,
        IEnumerable<PageRoleInfo> pageRoleInfos)
    {
        foreach (Role role in roles)
        {
            role.AppId = appId;
            role.Pages ??= [];
        }

        foreach (PageRoleInfo pageRoleInfo in pageRoleInfos)
        {
            Role role = roles.FirstOrDefault(predicate: candidate =>
                string.Equals(
                    a: candidate.Name,
                    b: pageRoleInfo.Role,
                    comparisonType: StringComparison.OrdinalIgnoreCase));

            if (role is null)
            {
                throw new ArgumentException(
                    message: $"Role '{pageRoleInfo.Role}' was not available for page-role import.");
            }

            role.Pages.Add(item: new PageRole
            {
                Page = new Page { AppId = appId, Path = pageRoleInfo.Path },
            });
        }
    }
}