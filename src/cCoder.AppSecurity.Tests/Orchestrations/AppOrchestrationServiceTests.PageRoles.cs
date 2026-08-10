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
    public async Task ShouldRetainRootPageRelationshipForGeneratedGuestRoleAsync()
    {
        // Given
        const int appId = 7;
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
                new Role
                {
                    Name = "Guests",
                    Pages =
                    [
                        new PageRole
                        {
                            Page = new Page
                            {
                                AppId = appId,
                                Path = string.Empty,
                            },
                        },
                    ],
                },
            ],
        };

        authorizationServiceMock
            .Setup(expression: service => service.GetCurrentUser())
            .Returns(value: null as User);

        privilegeServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[]
            {
                new Privilege
                {
                    Id = "page_read",
                    Operation = "Read",
                    Type = "Page",
                },
            }.AsQueryable());

        roleServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<Role>()
                .AsQueryable());

        roleServiceMock
            .Setup(expression: service => service.AddValidatedRoleAsync(
                role: It.IsAny<Role>()))
            .ReturnsAsync(valueFunction: (Role role) => role);

        // When
        await orchestrationService.AddAppAsync(newApp: app);

        // Then
        roleServiceMock.Verify(
            expression: service => service.AddValidatedRoleAsync(
                role: It.Is<Role>(match: role =>
                    role.Name == "Guests"
                    && role.Pages.Count == 1
                    && role.Pages.Single().Page.Path == string.Empty)),
            times: Times.Once);
    }
}