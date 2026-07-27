// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AppOrchestrationService(
    IAuthorizationService authorizationService,
    IPrivilegeService privilegeService,
    IRoleService roleService
) : IAppOrchestrationService
{
    public ValueTask AddAppAsync(App newApp) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateAddApp(
                newApp: newApp);

            EnsureDefaultRoles(app: newApp);
            StampRoles(app: newApp);
            await UpsertRolesAsync(roles: newApp.Roles ?? []);

        });

    public ValueTask UpdateAppAsync(App updatedApp) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateUpdateApp(
                updatedApp: updatedApp);

            if (updatedApp?.Roles == null || updatedApp.Roles.Count == 0)
            {
                return;
            }

            StampRoles(app: updatedApp);
            await UpsertRolesAsync(roles: updatedApp.Roles);

        });

    public ValueTask DeleteAsync(int appId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDelete(
                appId: appId);

            Role[] rolesToDelete = [.. roleService.GetAll(ignoreFilters: true)
                .Where(predicate: role => role.AppId == appId)];

            foreach (Role role in rolesToDelete)
            {
                await roleService.DeleteValidatedAsync(id: role.Id);
            }

        });

    private static void StampRoles(App app)
    {
        foreach (Role role in app.Roles ?? [])
        {
            role.AppId = app.Id;
            role.App = app;
        }
    }

    private async ValueTask UpsertRolesAsync(IEnumerable<Role> roles)
    {
        Role[] roleArray =
            [.. roles.OrderBy(keySelector: GetBootstrapOrder)
                .ThenBy(keySelector: role => role.Name, comparer: StringComparer.OrdinalIgnoreCase)];

        int? appId = roleArray
            .Select(selector: role => (int?)role.AppId)
            .FirstOrDefault();

        Dictionary<string, Role> existingRolesByName = roleService
            .GetAll(ignoreFilters: true)
            .Where(predicate: foundRole =>
                appId.HasValue
                && foundRole.AppId == appId.Value
                && !string.IsNullOrWhiteSpace(value: foundRole.Name))
            .ToArray()
            .GroupBy(
                keySelector: foundRole => foundRole.Name,
                comparer: StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                keySelector: group => group.Key,
                elementSelector: group => group.First(),
                comparer: StringComparer.OrdinalIgnoreCase);

        foreach (Role role in roleArray)
        {
            if (existingRolesByName.TryGetValue(key: role.Name, value: out Role existingRole))
            {
                role.Id = existingRole.Id;

                foreach (UserRole userRole in role.Users ?? [])
                {
                    userRole.RoleId = existingRole.Id;
                    userRole.Role = role;
                }

                _ = await roleService.UpdateValidatedRoleAsync(role: role);
            }
            else
            {
                _ = await roleService.AddValidatedRoleAsync(role: role);
            }
        }
    }

    private static int GetBootstrapOrder(Role role) =>
        role.Name?.ToLowerInvariant() switch
        {
            "administrators" => 0,
            "users" => 1,
            "guests" => 2,
            _ => 3,
        };

    private void EnsureDefaultRoles(App app)
    {
        string[] builtInRoleNames = ["Administrators", "Users", "Guests"];

        app.Roles = [.. (app.Roles ?? [])
            .Where(predicate: role => !builtInRoleNames.Contains(
                value: role.Name,
                comparer: StringComparer.OrdinalIgnoreCase))];

        string currentUserId = authorizationService.GetCurrentUser()?.Id;
        Privilege[] privileges = [.. privilegeService.GetAll(ignoreFilters: true)];

        string[] administratorPrivileges =
            [.. privileges
                .Where(predicate: privilege => privilege.Id != "app_create")
                .Select(selector: privilege => privilege.Id)];

        string[] userPrivileges =
            [.. privileges
                .Where(predicate: privilege =>
                    string.Equals(a: privilege.Operation, b: "Read", comparisonType: StringComparison.OrdinalIgnoreCase)
                    && !IsWorkflowType(type: privilege.Type))
                .Select(selector: privilege => privilege.Id)];

        AddBuiltInRole(newApp: app, roleName: "Administrators", privileges: administratorPrivileges, userId: currentUserId);
        AddBuiltInRole(newApp: app, roleName: "Users", privileges: userPrivileges, userId: currentUserId);
        AddBuiltInRole(newApp: app, roleName: "Guests", privileges: userPrivileges, userId: "Guest");
    }

    private static bool IsWorkflowType(string type) =>
        type.StartsWith(value: "Flow", comparisonType: StringComparison.OrdinalIgnoreCase)
        || type.StartsWith(value: "Workflow", comparisonType: StringComparison.OrdinalIgnoreCase);

    private static void AddBuiltInRole(
        App newApp,
        string roleName,
        IEnumerable<string> privileges,
        string userId)
    {
        Role newRole = new()
        {
            Id = Guid.NewGuid(),
            AppId = newApp.Id,
            App = newApp,
            Name = roleName,
            Users = [],
            Pages = [],
            Folders = [],
            Privileges = [.. privileges],
        };

        if (!string.IsNullOrWhiteSpace(value: userId))
        {
            newRole.Users.Add(item: new UserRole
            {
                RoleId = newRole.Id,
                UserId = userId,
                Role = newRole,
            });
        }

        newApp.Roles.Add(item: newRole);
    }
}