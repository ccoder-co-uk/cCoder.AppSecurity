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
        EnsureDefaultRoles(app);
        StampRoles(app);
        await UpsertRolesAsync(app.Roles ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        if (app?.Roles == null || app.Roles.Count == 0)
        {
            return;
        }

        StampRoles(app);
        await UpsertRolesAsync(app.Roles);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        Role[] rolesToDelete = [.. roleOrchestrationService.GetAll(true).Where(role => role.AppId == appId)];

        foreach (Role role in rolesToDelete)
            await roleOrchestrationService.DeleteAsync(role.Id);
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
            [.. roles.OrderBy(GetBootstrapOrder)
                .ThenBy(role => role.Name, StringComparer.OrdinalIgnoreCase)];
        Guid[] roleIds = [.. roleArray.Select(role => role.Id)];
        HashSet<Guid> existingRoleIds =
            [.. roleOrchestrationService.GetAll(true)
                .Where(foundRole => roleIds.Contains(foundRole.Id))
                .Select(foundRole => foundRole.Id)];

        foreach (Role role in roleArray)
        {
            if (existingRoleIds.Contains(role.Id))
            {
                _ = await roleOrchestrationService.UpdateAsync(role);
            }
            else
            {
                _ = await roleOrchestrationService.AddAsync(role);
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
        Privilege[] privileges = [.. privilegeService.GetAll(true)];

        string[] administratorPrivileges =
            [.. privileges
                .Where(privilege => privilege.Id != "app_create")
                .Select(privilege => privilege.Id)];

        string[] userPrivileges =
            [.. privileges
                .Where(privilege =>
                    string.Equals(privilege.Operation, "Read", StringComparison.OrdinalIgnoreCase)
                    && !IsWorkflowType(privilege.Type))
                .Select(privilege => privilege.Id)];

        EnsureRole(app, "Administrators", administratorPrivileges, currentUserId);
        EnsureRole(app, "Users", userPrivileges, currentUserId);
        EnsureRole(app, "Guests", userPrivileges, "Guest");
    }

    private static bool IsWorkflowType(string type) =>
        type.StartsWith("Flow", StringComparison.OrdinalIgnoreCase)
        || type.StartsWith("Workflow", StringComparison.OrdinalIgnoreCase);

    private static void EnsureRole(
        App app,
        string roleName,
        IEnumerable<string> requiredPrivileges,
        string userId
    )
    {
        Role role = app.Roles.FirstOrDefault(foundRole =>
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

            app.Roles.Add(role);
        }

        role.AppId = app.Id;
        role.App ??= app;
        role.Users ??= [];
        role.Pages ??= [];
        role.Folders ??= [];
        role.Privileges = [.. role.Privileges.Union(requiredPrivileges, StringComparer.OrdinalIgnoreCase)];

        if (
            !string.IsNullOrWhiteSpace(userId)
            && !role.Users.Any(existingUserRole => existingUserRole.UserId == userId)
        )
        {
            role.Users.Add(new UserRole { RoleId = role.Id, UserId = userId, Role = role });
        }
    }
}

