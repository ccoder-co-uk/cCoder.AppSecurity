// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReuseExistingDefaultRolesWhenAddAppIsReplayedAsync()
    {
        // Given
        const int appId = 7;
        const string userId = "setup-admin";
        Guid postedAdministratorRoleId = Guid.NewGuid();
        Guid customRoleId = Guid.NewGuid();

        Mock<IAuthorizationService> authorizationServiceMock = new(behavior: MockBehavior.Strict);
        Mock<IPrivilegeService> privilegeServiceMock = new(behavior: MockBehavior.Strict);
        Mock<IRoleService> roleServiceMock = new(behavior: MockBehavior.Strict);

        AppOrchestrationService orchestrationService = new(
            authorizationService: authorizationServiceMock.Object,
            privilegeService: privilegeServiceMock.Object,
            roleService: roleServiceMock.Object);

        App app = new()
        {
            Id = appId,
            Roles =
            [
                new()
                {
                    Id = postedAdministratorRoleId,
                    Name = "administrators",
                    Privileges = ["untrusted_privilege"]
                },
                new()
                {
                    Id = customRoleId,
                    Name = "Reviewers",
                    Privileges = ["app_read"]
                }
            ]
        };

        Role[] existingRoles =
        [
            new() { Id = Guid.NewGuid(), AppId = appId, Name = "Administrators" },
            new() { Id = Guid.NewGuid(), AppId = appId, Name = "Users" },
            new() { Id = Guid.NewGuid(), AppId = appId, Name = "Guests" }
        ];

        IQueryable<Privilege> privileges = new[]
        {
            new Privilege { Id = "app_read", Operation = "Read", Type = "App" }
        }.AsQueryable();

        authorizationServiceMock
            .Setup(expression: service => service.GetCurrentUser())
            .Returns(value: new User { Id = userId });

        privilegeServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: privileges);

        roleServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: existingRoles.AsQueryable());

        roleServiceMock
            .Setup(expression: service => service.UpdateValidatedRoleAsync(
                role: It.IsAny<Role>()))
            .ReturnsAsync(value: new Role());

        roleServiceMock
            .Setup(expression: service => service.AddValidatedRoleAsync(
                role: It.IsAny<Role>()))
            .ReturnsAsync(value: new Role());

        // When
        await orchestrationService.AddAppAsync(newApp: app);

        // Then
        foreach (Role existingRole in existingRoles)
        {
            roleServiceMock.Verify(
                expression: service => service.UpdateValidatedRoleAsync(
                    role: It.Is<Role>(match: role =>
                        role.Name == existingRole.Name
                        && role.Id == existingRole.Id
                        && role.Id != postedAdministratorRoleId
                        && !role.Privileges.Contains(value: "untrusted_privilege"))),
                times: Times.Once);
        }

        roleServiceMock.Verify(
            expression: service => service.AddValidatedRoleAsync(
                role: It.Is<Role>(match: role =>
                    role.Id == customRoleId
                    && role.AppId == appId
                    && role.Name == "Reviewers")),
            times: Times.Once);
    }
}