// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IUserBroker = cCoder.AppSecurity.Brokers.Storages.IUserBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserServiceTests
{
    private readonly Mock<IUserBroker> userBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly UserService userService;

    public UserServiceTests()
    {
        userBrokerMock = new Mock<IUserBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        userService = new UserService(userBrokerMock.Object, authorizationBrokerMock.Object);
    }

    private static User CreateRandomUser(string id = null)
    {
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Id = id ?? $"user-{Guid.NewGuid():N}")
            .With(x => x.DefaultCultureId = "en-GB")
            .With(x => x.DisplayName = $"Display-{Guid.NewGuid():N}")
            .With(x => x.Email = $"user-{Guid.NewGuid():N}@test.local")
            .With(x => x.IsActive = true)
            .With(x => x.Roles = Array.Empty<UserRole>())
            .Build();

        return user;
    }

    private static DataUser ToExternalUser(User item) =>
        item == null
            ? null
            : new DataUser
            {
                Id = item.Id,
                DefaultCultureId = item.DefaultCultureId,
                DisplayName = item.DisplayName,
                Email = item.Email,
                IsActive = item.IsActive,
                DefaultCulture = item.DefaultCulture as cCoder.Data.Models.CMS.Culture,
                Roles = item.Roles?.Select(userRole => new DataUserRole
                {
                    RoleId = userRole.RoleId,
                    UserId = userRole.UserId,
                    Role = userRole.Role == null ? null : new DataRole
                    {
                        Id = userRole.Role.Id,
                        AppId = userRole.Role.AppId,
                        Name = userRole.Role.Name,
                        Description = userRole.Role.Description,
                        Privs = userRole.Role.Privs,
                    },
                }).ToArray(),
            };
}