// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataApp = cCoder.Data.Models.CMS.App;
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;
using DataPageRole = cCoder.Data.Models.Security.PageRole;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IRoleBroker = cCoder.AppSecurity.Brokers.IRoleBroker;
using IUserRoleBroker = cCoder.AppSecurity.Brokers.Storages.IUserRoleBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    private readonly Mock<IRoleBroker> roleBrokerMock;
    private readonly Mock<IUserRoleBroker> userRoleBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly RoleService roleService;

    public RoleServiceTests()
    {
        roleBrokerMock = new Mock<IRoleBroker>(behavior: MockBehavior.Strict);
        userRoleBrokerMock = new Mock<IUserRoleBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);
        roleService = new RoleService(roleBroker: roleBrokerMock.Object, userRoleBroker: userRoleBrokerMock.Object, authorizationBroker: authorizationBrokerMock.Object);
    }

    private static Role CreateRandomRole(Guid? id = null, int appId = 7)
    {
        Role role = Builder<Role>
            .CreateNew()
            .With(func: x => x.Id = id ?? Guid.NewGuid())
            .With(func: x => x.AppId = appId)
            .With(func: x => x.Name = $"Role-{Guid.NewGuid():N}")
            .With(func: x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(func: x => x.Privs = "page_read,page_write")
            .With(func: x => x.Users = Array.Empty<UserRole>())
            .With(func: x => x.Pages = Array.Empty<cCoder.Data.Models.Security.PageRole>())
            .With(func: x => x.Folders = Array.Empty<cCoder.Data.Models.Security.FolderRole>())
            .Build();

        return role;
    }

    private static DataUser CreateCurrentUser(int appId, params string[] privileges)
    {
        DataRole role = new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = $"CurrentUserRole-{Guid.NewGuid():N}",
            Privs = string.Join(separator: ',', value: privileges),
        };

        return new DataUser
        {
            Id = "current-user",
            Roles =
            [
                new DataUserRole
                {
                    UserId = "current-user",
                    RoleId = role.Id,
                    Role = role,
                }
            ],
        };
    }

    private static DataRole ToExternalRole(Role item) =>
        item == null
            ? null
            : new DataRole
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
                Users = item.Users?.Select(selector: ToExternalUserRole).ToArray(),
                Pages = item.Pages?.Select(selector: pageRole => new DataPageRole
                {
                    PageId = pageRole.PageId,
                    RoleId = pageRole.RoleId,
                }).ToArray(),
                Folders = item.Folders?.Select(selector: folderRole => new DataFolderRole
                {
                    FolderId = folderRole.FolderId,
                    RoleId = folderRole.RoleId,
                }).ToArray(),
            };

    private static DataUserRole ToExternalUserRole(UserRole item) =>
        item == null
            ? null
            : new DataUserRole
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
            };
}