using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldNotAssignGuestToBootstrapAdministratorRoleForFirstApp()
    {
        Mock<IAuthorizationBroker> authorizationBrokerMock = new(MockBehavior.Strict);
        Mock<IPrivilegeService> privilegeServiceMock = new(MockBehavior.Strict);
        Mock<IRoleOrchestrationService> roleOrchestrationServiceMock = new(MockBehavior.Strict);
        Mock<IUserRoleOrchestrationService> userRoleOrchestrationServiceMock = new(MockBehavior.Strict);
        List<Role> savedRoles = [];

        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new User { Id = "Guest" });

        privilegeServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[]
            {
                new Privilege { Id = "app_admin", Type = "App", Operation = "Admin", Description = "App admin" },
                new Privilege { Id = "app_create", Type = "App", Operation = "Create", Description = "Create app" },
                new Privilege { Id = "page_read", Type = "Page", Operation = "Read", Description = "Read page" },
                new Privilege { Id = "workflowevent_read", Type = "WorkflowEvent", Operation = "Read", Description = "Read workflow event" },
            }.AsQueryable());

        roleOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(Array.Empty<Role>().AsQueryable());

        userRoleOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(Array.Empty<UserRole>().AsQueryable());

        roleOrchestrationServiceMock
            .Setup(x => x.AddValidatedAsync(It.IsAny<Role>()))
            .Callback<Role>(role => savedRoles.Add(role))
            .ReturnsAsync((Role role) => role);

        AppOrchestrationService orchestrationService = new(
            authorizationBrokerMock.Object,
            privilegeServiceMock.Object,
            roleOrchestrationServiceMock.Object,
            userRoleOrchestrationServiceMock.Object);

        await orchestrationService.AddAsync(new App { Id = 1, Roles = [] });

        Role administrators = savedRoles.Single(role => role.Name == "Administrators");
        Role users = savedRoles.Single(role => role.Name == "Users");
        Role guests = savedRoles.Single(role => role.Name == "Guests");

        administrators.Privileges.Should().Contain("app_create");
        administrators.Users.Should().NotContain(userRole => userRole.UserId == "Guest");
        users.Users.Should().NotContain(userRole => userRole.UserId == "Guest");
        guests.Users.Should().ContainSingle(userRole => userRole.UserId == "Guest");
        users.Privileges.Should().Contain("page_read");
        users.Privileges.Should().NotContain("workflowevent_read");
        roleOrchestrationServiceMock.Verify(x => x.AddValidatedAsync(It.IsAny<Role>()), Times.Exactly(3));
        roleOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.AtLeastOnce());
        roleOrchestrationServiceMock.VerifyNoOtherCalls();
        userRoleOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAssignCurrentUserAndExcludeAppCreateWhenAppSecurityAlreadyExists()
    {
        Mock<IAuthorizationBroker> authorizationBrokerMock = new(MockBehavior.Strict);
        Mock<IPrivilegeService> privilegeServiceMock = new(MockBehavior.Strict);
        Mock<IRoleOrchestrationService> roleOrchestrationServiceMock = new(MockBehavior.Strict);
        Mock<IUserRoleOrchestrationService> userRoleOrchestrationServiceMock = new(MockBehavior.Strict);
        List<Role> savedRoles = [];

        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new User { Id = "paul" });

        privilegeServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[]
            {
                new Privilege { Id = "app_admin", Type = "App", Operation = "Admin", Description = "App admin" },
                new Privilege { Id = "app_create", Type = "App", Operation = "Create", Description = "Create app" },
                new Privilege { Id = "page_read", Type = "Page", Operation = "Read", Description = "Read page" },
            }.AsQueryable());

        roleOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[]
            {
                new Role { Id = Guid.NewGuid(), AppId = 99, Name = "Existing", Privs = "app_read" }
            }.AsQueryable());

        userRoleOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[]
            {
                new UserRole
                {
                    RoleId = Guid.NewGuid(),
                    UserId = "paul"
                }
            }.AsQueryable());

        roleOrchestrationServiceMock
            .Setup(x => x.AddAsync(It.IsAny<Role>()))
            .Callback<Role>(role => savedRoles.Add(role))
            .ReturnsAsync((Role role) => role);

        AppOrchestrationService orchestrationService = new(
            authorizationBrokerMock.Object,
            privilegeServiceMock.Object,
            roleOrchestrationServiceMock.Object,
            userRoleOrchestrationServiceMock.Object);

        await orchestrationService.AddAsync(new App { Id = 2, Roles = [] });

        Role administrators = savedRoles.Single(role => role.Name == "Administrators");
        Role users = savedRoles.Single(role => role.Name == "Users");
        Role guests = savedRoles.Single(role => role.Name == "Guests");

        administrators.Privileges.Should().NotContain("app_create");
        administrators.Users.Should().ContainSingle(userRole => userRole.UserId == "paul");
        users.Users.Should().ContainSingle(userRole => userRole.UserId == "paul");
        guests.Users.Should().ContainSingle(userRole => userRole.UserId == "Guest");
        roleOrchestrationServiceMock.Verify(x => x.AddAsync(It.IsAny<Role>()), Times.Exactly(3));
        roleOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.AtLeastOnce());
        roleOrchestrationServiceMock.VerifyNoOtherCalls();
        userRoleOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}
