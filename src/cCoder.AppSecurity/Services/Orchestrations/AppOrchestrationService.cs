// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal class AppOrchestrationService(
    IAuthorizationBroker authorizationBroker,
    IPrivilegeService privilegeService,
    IRoleOrchestrationService roleOrchestrationService
) : IAppOrchestrationService
{
    public async ValueTask AddAsync(App app)
    {
        EnsureDefaultRoles(app: app);
        StampRoles(app: app);
        await UpsertRolesAsync(roles: app.Roles ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        if (app?.Roles == null || app.Roles.Count == 0)
        {
            return;
        }

        StampRoles(app: app);
        await UpsertRolesAsync(roles: app.Roles);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        Role[] rolesToDelete = [.. roleOrchestrationService.GetAll(ignoreFilters: true).Where(predicate: role => role.AppId == appId)];

        foreach (Role role in rolesToDelete)
            await roleOrchestrationService.DeleteValidatedAsync(id: role.Id);
    }

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
        Guid[] roleIds = [.. roleArray.Select(selector: role => role.Id)];
        HashSet<Guid> existingRoleIds =
            [.. roleOrchestrationService.GetAll(ignoreFilters: true)
                .Where(predicate: foundRole => roleIds.Contains(foundRole.Id))
                .Select(selector: foundRole => foundRole.Id)];

        foreach (Role role in roleArray)
        {
            if (existingRoleIds.Contains(item: role.Id))
            {
                _ = await roleOrchestrationService.UpdateValidatedAsync(entity: role);
            }
            else
            {
                _ = await roleOrchestrationService.AddValidatedAsync(entity: role);
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
        app.Roles ??= [];

        string currentUserId = authorizationBroker.GetCurrentUser()?.Id;
        Privilege[] privileges = [.. privilegeService.GetAll(ignoreFilters: true)];

        string[] administratorPrivileges =
            [.. privileges
                .Where(predicate: privilege => privilege.Id != "app_create")
                .Select(selector: privilege => privilege.Id)];

        string[] userPrivileges =
            [.. privileges
                .Where(predicate: privilege =>
                    string.Equals(privilege.Operation, "Read", StringComparison.OrdinalIgnoreCase)
                    && !IsWorkflowType(privilege.Type))
                .Select(selector: privilege => privilege.Id)];

        EnsureRole(app: app, roleName: "Administrators", requiredPrivileges: administratorPrivileges, userId: currentUserId);
        EnsureRole(app: app, roleName: "Users", requiredPrivileges: userPrivileges, userId: currentUserId);
        EnsureRole(app: app, roleName: "Guests", requiredPrivileges: userPrivileges, userId: "Guest");
    }

    private static bool IsWorkflowType(string type) =>
        type.StartsWith(value: "Flow", comparisonType: StringComparison.OrdinalIgnoreCase)
        || type.StartsWith(value: "Workflow", comparisonType: StringComparison.OrdinalIgnoreCase);

    private static void EnsureRole(
        App app,
        string roleName,
        IEnumerable<string> requiredPrivileges,
        string userId
    )
    {
        Role role = app.Roles.FirstOrDefault(predicate: foundRole =>
            string.Equals(foundRole.Name, roleName, StringComparison.OrdinalIgnoreCase));

        if (role is null)
        {
            role = new Role
            {
                Id = Guid.NewGuid(),
                AppId = app.Id,
                App = app,
                Name = roleName,
                Users = [],
                Pages = [],
                Folders = [],
                Privileges = [],
            };

            app.Roles.Add(item: role);
        }

        role.AppId = app.Id;
        role.App ??= app;
        role.Users ??= [];
        role.Pages ??= [];
        role.Folders ??= [];
        role.Privileges = [.. role.Privileges.Union(second: requiredPrivileges, comparer: StringComparer.OrdinalIgnoreCase)];

        if (
            !string.IsNullOrWhiteSpace(value: userId)
            && !role.Users.Any(predicate: existingUserRole => existingUserRole.UserId == userId)
        )
        {
            role.Users.Add(item: new UserRole { RoleId = role.Id, UserId = userId, Role = role });
        }
    }
}