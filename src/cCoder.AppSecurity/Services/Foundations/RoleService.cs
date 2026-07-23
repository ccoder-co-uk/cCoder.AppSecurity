// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using DataApp = cCoder.Data.Models.CMS.App;
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;
using DataPageRole = cCoder.Data.Models.Security.PageRole;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IRoleBroker = cCoder.AppSecurity.Brokers.IRoleBroker;
using IUserRoleBroker = cCoder.AppSecurity.Brokers.Storages.IUserRoleBroker;


namespace cCoder.AppSecurity.Services.Foundations;

internal class RoleService(
    IRoleBroker roleBroker,
    IUserRoleBroker userRoleBroker,
    IAuthorizationBroker authorizationBroker)
    : IRoleService
{
    public Role Get(Guid id)
    {
        Role role = GetAll().FirstOrDefault(i => i.Id == id);
        if (role is not null)
            return role;

        Role unrestrictedRole = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedRole is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        roleBroker.GetAllRoles(ignoreFilters);

    public async ValueTask<Role> AddAsync(Role role)
    {
        DataRole internalRole = new()
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs
        };
        AuthorizeMutationOrAllowBootstrap(role.AppId, $"{nameof(Role)}_create", role.Privs);
        DataRole result = await roleBroker.AddRoleAsync(internalRole);
        role.Id = result.Id;
        role.AppId = result.AppId;
        role.Name = result.Name;
        role.Description = result.Description;
        role.Privs = result.Privs;
        return role;
    }

    public async ValueTask<Role> AddValidatedAsync(Role role)
    {
        DataRole internalRole = new()
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs
        };
        DataRole result = await roleBroker.AddRoleAsync(internalRole);
        role.Id = result.Id;
        role.AppId = result.AppId;
        role.Name = result.Name;
        role.Description = result.Description;
        role.Privs = result.Privs;
        return role;
    }

    public async ValueTask<Role> UpdateAsync(Role role)
    {
        DataRole internalRole = new()
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs
        };
        AuthorizeMutationOrAllowBootstrap(role.AppId, $"{nameof(Role)}_update", role.Privs);
        DataRole result = await roleBroker.UpdateRoleAsync(internalRole);
        role.Id = result.Id;
        role.AppId = result.AppId;
        role.Name = result.Name;
        role.Description = result.Description;
        role.Privs = result.Privs;
        return role;
    }

    public async ValueTask<Role> UpdateValidatedAsync(Role role)
    {
        DataRole internalRole = new()
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs
        };
        DataRole result = await roleBroker.UpdateRoleAsync(internalRole);
        role.Id = result.Id;
        role.AppId = result.AppId;
        role.Name = result.Name;
        role.Description = result.Description;
        role.Privs = result.Privs;
        return role;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        Role role = GetAll(true).FirstOrDefault(foundRole => foundRole.Id == id);

        if (role is null)
            return;

        authorizationBroker.Authorize(role.AppId, $"{nameof(Role)}_delete");
        await DeleteRoleAsync(role);
    }

    public async ValueTask DeleteValidatedAsync(Guid id)
    {
        Role role = GetAll(true).FirstOrDefault(foundRole => foundRole.Id == id);

        if (role is null)
            return;

        await DeleteRoleAsync(role);
    }

    private async ValueTask DeleteRoleAsync(Role role)
    {
        DataUserRole[] userRoles = [.. userRoleBroker.GetAllUserRoles(true).Where(userRole => userRole.RoleId == role.Id)];

        if (userRoles.Length > 0)
            await userRoleBroker.DeleteAllUserRolesAsync(userRoles);

        await roleBroker.DeletePageRolesByRoleIdAsync(role.Id);
        await roleBroker.DeleteFolderRolesByRoleIdAsync(role.Id);
        _ = await roleBroker.DeleteRoleAsync(ToExternalRole(role));
    }

    private void AuthorizeMutationOrAllowBootstrap(int? appId, string privilege, string assignedPrivileges)
    {
        if (!HasAnyRoles(appId))
        {
            return;
        }

        authorizationBroker.Authorize(appId, privilege);
        AuthorizeAssignedPrivileges(appId, assignedPrivileges);
    }

    private void AuthorizeAssignedPrivileges(int? appId, string assignedPrivileges)
    {
        if (!appId.HasValue)
            return;

        string[] assignedPrivilegeSet = ToPrivilegeSet(assignedPrivileges);

        if (assignedPrivilegeSet.Length == 0)
            return;

        User currentUser = authorizationBroker.GetCurrentUser();
        HashSet<string> grantedPrivileges = currentUser?.Roles?
            .Where(userRole => userRole.Role?.AppId == appId.Value)
            .SelectMany(userRole => ToPrivilegeSet(userRole.Role.Privs))
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? [];

        if (assignedPrivilegeSet.Any(assignedPrivilege => !grantedPrivileges.Contains(assignedPrivilege)))
            throw new SecurityException("Access Denied!");
    }

    private bool HasAnyRoles(int? appId) =>
        appId.HasValue
        && roleBroker.GetAllRoles(ignoreFilters: true)
            .Any(foundRole => foundRole.AppId == appId.Value);

    private static string[] ToPrivilegeSet(string privileges) =>
        string.IsNullOrWhiteSpace(privileges)
            ? []
            : [.. privileges
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(privilege => privilege.ToLowerInvariant())
                .Distinct()];

    static Role ToLocalRole(DataRole item) =>
        new()
        {
            Id = item.Id,
            AppId = item.AppId,
            Name = item.Name,
            Description = item.Description,
            Privs = item.Privs,
            App = item.App == null ? null : ToLocalApp(item.App),
            Users = item.Users?.Select(ToLocalUserRole).ToArray(),
            Pages = item.Pages?.Select(ToLocalPageRole).ToArray(),
            Folders = item.Folders?.Select(ToLocalFolderRole).ToArray(),
        };

    static DataRole ToExternalRole(Role item) =>
        new()
        {
            Id = item.Id,
            AppId = item.AppId,
            Name = item.Name,
            Description = item.Description,
            Privs = item.Privs,
            App = null,
            Users = item.Users?.Select(ToExternalUserRole).ToArray(),
            Pages = item.Pages?.Select(ToExternalPageRole).ToArray(),
            Folders = item.Folders?.Select(ToExternalFolderRole).ToArray(),
        };

    static App ToLocalApp(DataApp item) =>
        new()
        {
            Id = item.Id,
            DefaultCultureId = item.DefaultCultureId,
            TenantId = item.TenantId,
            Name = item.Name,
            Domain = item.Domain,
            DefaultTheme = item.DefaultTheme,
            ConfigJson = item.ConfigJson,
        };

    static DataApp ToExternalApp(App item) =>
        new()
        {
            Id = item.Id,
            DefaultCultureId = item.DefaultCultureId,
            TenantId = item.TenantId,
            Name = item.Name,
            Domain = item.Domain,
            DefaultTheme = item.DefaultTheme,
            ConfigJson = item.ConfigJson,
        };

    static UserRole ToLocalUserRole(DataUserRole item) =>
        new()
        {
            RoleId = item.RoleId,
            UserId = item.UserId,
            User = item.User == null ? null : new User
            {
                Id = item.User.Id,
                DefaultCultureId = item.User.DefaultCultureId,
                DisplayName = item.User.DisplayName,
                Email = item.User.Email,
                IsActive = item.User.IsActive,
                DefaultCulture = item.User.DefaultCulture,
            },
            Role = null,
        };

    static DataUserRole ToExternalUserRole(UserRole item) =>
        new()
        {
            RoleId = item.RoleId,
            UserId = item.UserId,
            User = item.User == null ? null : new DataUser
            {
                Id = item.User.Id,
                DefaultCultureId = item.User.DefaultCultureId,
                DisplayName = item.User.DisplayName,
                Email = item.User.Email,
                IsActive = item.User.IsActive,
                DefaultCulture = item.User.DefaultCulture as cCoder.Data.Models.CMS.Culture,
            },
            Role = null,
        };

    static PageRole ToLocalPageRole(DataPageRole item) =>
        new()
        {
            PageId = item.PageId,
            RoleId = item.RoleId,
        };

    static DataPageRole ToExternalPageRole(PageRole item) =>
        new()
        {
            PageId = item.PageId,
            RoleId = item.RoleId,
        };

    static FolderRole ToLocalFolderRole(DataFolderRole item) =>
        new()
        {
            FolderId = item.FolderId,
            RoleId = item.RoleId,
        };

    static DataFolderRole ToExternalFolderRole(FolderRole item) =>
        new()
        {
            FolderId = item.FolderId,
            RoleId = item.RoleId,
        };
}