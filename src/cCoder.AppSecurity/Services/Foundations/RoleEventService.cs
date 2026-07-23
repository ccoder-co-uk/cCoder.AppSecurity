// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data;
using cCoder.Eventing.Models;
using DataApp = cCoder.Data.Models.CMS.App;
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;
using DataPageRole = cCoder.Data.Models.Security.PageRole;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IRoleEventBroker = cCoder.AppSecurity.Brokers.Events.IRoleEventBroker;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal class RoleEventService(IRoleEventBroker roleEventBroker, ICoreAuthInfo authInfo)
    : IRoleEventService
{
    public async ValueTask RaiseRoleAddEventAsync(Role entity)
    {
        EventMessage<DataRole> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalRole(entity),
        };

        await roleEventBroker.RaiseRoleAddEventAsync(message);
    }

    public async ValueTask RaiseRoleUpdateEventAsync(Role entity)
    {
        EventMessage<DataRole> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalRole(entity),
        };

        await roleEventBroker.RaiseRoleUpdateEventAsync(message);
    }

    public async ValueTask RaiseRoleDeleteEventAsync(Role entity)
    {
        EventMessage<DataRole> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalRole(entity),
        };

        await roleEventBroker.RaiseRoleDeleteEventAsync(message);
    }

    static DataRole ToExternalRole(Role item) =>
        new()
        {
            Id = item.Id,
            AppId = item.AppId,
            Name = item.Name,
            Description = item.Description,
            Privs = item.Privs,
            App = item.App == null ? null : new DataApp
            {
                Id = item.App.Id,
                DefaultCultureId = item.App.DefaultCultureId,
                TenantId = item.App.TenantId,
                Name = item.App.Name,
                Domain = item.App.Domain,
                DefaultTheme = item.App.DefaultTheme,
                ConfigJson = item.App.ConfigJson,
            },
            Users = item.Users?.Select(userRole => new DataUserRole
            {
                RoleId = userRole.RoleId,
                UserId = userRole.UserId,
                User = userRole.User == null ? null : new DataUser
                {
                    Id = userRole.User.Id,
                    DefaultCultureId = userRole.User.DefaultCultureId,
                    DisplayName = userRole.User.DisplayName,
                    Email = userRole.User.Email,
                    IsActive = userRole.User.IsActive,
                    DefaultCulture = userRole.User.DefaultCulture as cCoder.Data.Models.CMS.Culture,
                },
            }).ToArray(),
            Pages = item.Pages?.Select(pageRole => new DataPageRole
            {
                PageId = pageRole.PageId,
                RoleId = pageRole.RoleId,
            }).ToArray(),
            Folders = item.Folders?.Select(folderRole => new DataFolderRole
            {
                FolderId = folderRole.FolderId,
                RoleId = folderRole.RoleId,
            }).ToArray(),
        };
}