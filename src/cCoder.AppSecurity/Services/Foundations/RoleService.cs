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

internal sealed partial class RoleService(
    IRoleBroker roleBroker,
    IUserRoleBroker userRoleBroker,
    IAuthorizationBroker authorizationBroker)
    : IRoleService
{
    public Role Get(Guid roleId) =>
        TryCatch(operation: Role () =>
        {
            ValidateGet(
                roleId: roleId);

            Role role = GetAll()
                .FirstOrDefault(predicate: i => i.Id == roleId);

            if (role is not null)
            {
                return role;
            }

            Role unrestrictedRole = GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: i => i.Id == roleId);

            if (unrestrictedRole is not null)
            {
                throw new SecurityException(message: "Access Denied!");
            }

            return null;

        });

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<Role> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return roleBroker.GetAllRoles(ignoreFilters: ignoreFilters);
        });

    public ValueTask<Role> AddRoleAsync(Role newRole) =>
        TryCatch(operation: async ValueTask<Role> () =>
        {
            ValidateAddRole(
                newRole: newRole);

            DataRole internalRole = new()
            {
                Id = newRole.Id,
                AppId = newRole.AppId,
                Name = newRole.Name,
                Description = newRole.Description,
                Privs = newRole.Privs
            };

            AuthorizeMutationOrAllowBootstrap(appId: newRole.AppId, privilege: $"{nameof(Role)}_create", assignedPrivileges: newRole.Privs);
            DataRole result = await roleBroker.AddRoleAsync(entity: internalRole);
            newRole.Id = result.Id;
            newRole.AppId = result.AppId;
            newRole.Name = result.Name;
            newRole.Description = result.Description;
            newRole.Privs = result.Privs;
            return newRole;

        });

    public ValueTask<Role> AddValidatedRoleAsync(Role newRole) =>
        TryCatch(operation: async ValueTask<Role> () =>
        {
            ValidateAddValidatedRole(
                newRole: newRole);

            DataRole internalRole = new()
            {
                Id = newRole.Id,
                AppId = newRole.AppId,
                Name = newRole.Name,
                Description = newRole.Description,
                Privs = newRole.Privs
            };

            DataRole result = await roleBroker.AddRoleAsync(entity: internalRole);
            newRole.Id = result.Id;
            newRole.AppId = result.AppId;
            newRole.Name = result.Name;
            newRole.Description = result.Description;
            newRole.Privs = result.Privs;
            return newRole;

        });

    public ValueTask<Role> UpdateRoleAsync(Role updatedRole) =>
        TryCatch(operation: async ValueTask<Role> () =>
        {
            ValidateUpdateRole(
                updatedRole: updatedRole);

            DataRole internalRole = new()
            {
                Id = updatedRole.Id,
                AppId = updatedRole.AppId,
                Name = updatedRole.Name,
                Description = updatedRole.Description,
                Privs = updatedRole.Privs
            };

            AuthorizeMutationOrAllowBootstrap(appId: updatedRole.AppId, privilege: $"{nameof(Role)}_update", assignedPrivileges: updatedRole.Privs);
            DataRole result = await roleBroker.UpdateRoleAsync(entity: internalRole);
            updatedRole.Id = result.Id;
            updatedRole.AppId = result.AppId;
            updatedRole.Name = result.Name;
            updatedRole.Description = result.Description;
            updatedRole.Privs = result.Privs;
            return updatedRole;

        });

    public ValueTask<Role> UpdateValidatedRoleAsync(Role updatedRole) =>
        TryCatch(operation: async ValueTask<Role> () =>
        {
            ValidateUpdateValidatedRole(
                updatedRole: updatedRole);

            DataRole internalRole = new()
            {
                Id = updatedRole.Id,
                AppId = updatedRole.AppId,
                Name = updatedRole.Name,
                Description = updatedRole.Description,
                Privs = updatedRole.Privs
            };

            DataRole result = await roleBroker.UpdateRoleAsync(entity: internalRole);
            updatedRole.Id = result.Id;
            updatedRole.AppId = result.AppId;
            updatedRole.Name = result.Name;
            updatedRole.Description = result.Description;
            updatedRole.Privs = result.Privs;
            return updatedRole;

        });

    public ValueTask DeleteAsync(Guid roleId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDelete(
                roleId: roleId);

            Role role = GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: foundRole => foundRole.Id == roleId);

            if (role is null)
            {
                return;
            }

            authorizationBroker.Authorize(appId: role.AppId, privilege: $"{nameof(Role)}_delete");
            await DeleteRoleAsync(deletedRole: role);

        });

    public ValueTask DeleteValidatedAsync(Guid roleId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDeleteValidated(
                roleId: roleId);

            Role role = GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: foundRole => foundRole.Id == roleId);

            if (role is null)
            {
                return;
            }

            await DeleteRoleAsync(deletedRole: role);

        });

    private async ValueTask DeleteRoleAsync(Role deletedRole)
    {
        DataUserRole[] userRoles = [.. userRoleBroker.GetAllUserRoles(ignoreFilters: true)
            .Where(predicate: userRole => userRole.RoleId == deletedRole.Id)];

        if (userRoles.Length > 0)
        {
            await userRoleBroker.DeleteAllUserRolesAsync(items: userRoles);
        }

        await roleBroker.DeletePageRolesByRoleIdAsync(roleId: deletedRole.Id);
        await roleBroker.DeleteFolderRolesByRoleIdAsync(roleId: deletedRole.Id);
        _ = await roleBroker.DeleteRoleAsync(entity: ToExternalRole(item: deletedRole));
    }

    private void AuthorizeMutationOrAllowBootstrap(int? appId, string privilege, string assignedPrivileges)
    {
        if (!HasAnyRoles(appId: appId))
        {
            return;
        }

        authorizationBroker.Authorize(appId: appId, privilege: privilege);
        AuthorizeAssignedPrivileges(appId: appId, assignedPrivileges: assignedPrivileges);
    }

    private void AuthorizeAssignedPrivileges(int? appId, string assignedPrivileges)
    {
        if (!appId.HasValue)
        {
            return;
        }

        string[] assignedPrivilegeSet = ToPrivilegeSet(privileges: assignedPrivileges);

        if (assignedPrivilegeSet.Length == 0)
        {
            return;
        }

        User currentUser = authorizationBroker.GetCurrentUser();

        HashSet<string> grantedPrivileges = currentUser?.Roles?
            .Where(predicate: userRole => userRole.Role?.AppId == appId.Value)
            .SelectMany(selector: userRole => ToPrivilegeSet(privileges: userRole.Role.Privs))
            .ToHashSet(comparer: StringComparer.OrdinalIgnoreCase)
            ?? [];

        if (assignedPrivilegeSet.Any(predicate: assignedPrivilege => !grantedPrivileges.Contains(item: assignedPrivilege)))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    private bool HasAnyRoles(int? appId) =>
        appId.HasValue
        && roleBroker.GetAllRoles(ignoreFilters: true)
            .Any(predicate: foundRole => foundRole.AppId == appId.Value);

    private static string[] ToPrivilegeSet(string privileges) =>
        string.IsNullOrWhiteSpace(value: privileges)
            ? []
            : [.. privileges
                .Split(separator: ',', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(selector: privilege => privilege.ToLowerInvariant())
                .Distinct()];

    static Role ToLocalRole(DataRole item) =>
        new()
        {
            Id = item.Id,
            AppId = item.AppId,
            Name = item.Name,
            Description = item.Description,
            Privs = item.Privs,
            App = item.App == null ? null : ToLocalApp(item: item.App),
            Users = item.Users?.Select(selector: ToLocalUserRole)
                .ToArray(),
            Pages = item.Pages?.Select(selector: ToLocalPageRole)
                .ToArray(),
            Folders = item.Folders?.Select(selector: ToLocalFolderRole)
                .ToArray(),
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
            Users = item.Users?.Select(selector: ToExternalUserRole)
                .ToArray(),
            Pages = item.Pages?.Select(selector: ToExternalPageRole)
                .ToArray(),
            Folders = item.Folders?.Select(selector: ToExternalFolderRole)
                .ToArray(),
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