using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using Moq;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IRoleBroker = cCoder.AppSecurity.Brokers.IRoleBroker;
using IUserRoleBroker = cCoder.AppSecurity.Brokers.Storages.IUserRoleBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserRoleServiceTests
{
    private readonly Mock<IUserRoleBroker> userRoleBrokerMock;
    private readonly Mock<IRoleBroker> roleBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly UserRoleService userRoleService;

    public UserRoleServiceTests()
    {
        userRoleBrokerMock = new Mock<IUserRoleBroker>(MockBehavior.Strict);
        roleBrokerMock = new Mock<IRoleBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        userRoleService = new UserRoleService(
            userRoleBrokerMock.Object,
            roleBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static UserRole CreateRandomUserRole(Guid? roleId = null, string userId = null)
    {
        UserRole userRole = new()
        {
            RoleId = roleId ?? Guid.NewGuid(),
            UserId = userId ?? $"user-{Guid.NewGuid():N}",
            User = null!,
            Role = null!,
        };

        return userRole;
    }

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
                Role = item.Role == null ? null : new DataRole
                {
                    Id = item.Role.Id,
                    AppId = item.Role.AppId,
                    Name = item.Role.Name,
                    Description = item.Role.Description,
                    Privs = item.Role.Privs,
                },
            };

    private static DataRole CreateRole(Guid roleId, int appId, params string[] privileges) =>
        new()
        {
            Id = roleId,
            AppId = appId,
            Name = $"Role-{Guid.NewGuid():N}",
            Privs = string.Join(',', privileges),
        };

    private static DataUser CreateCurrentUser(int appId, params string[] privileges)
    {
        DataRole role = CreateRole(Guid.NewGuid(), appId, privileges);

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
}














