// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteUserRolesBeforeRolesWhenDeleteAsync()
    {
        // Given
        Mock<IAuthorizationService> authorizationServiceMock = new(behavior: MockBehavior.Strict);
        Mock<IPrivilegeService> privilegeServiceMock = new(behavior: MockBehavior.Strict);
        Mock<IRoleService> roleServiceMock = new(behavior: MockBehavior.Strict);

        AppOrchestrationService orchestrationService = new(
            authorizationService: authorizationServiceMock.Object,
            privilegeService: privilegeServiceMock.Object,
            roleService: roleServiceMock.Object);

        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 5,
            Name = "Administrators",
            Privs = "app_delete"
        };

        roleServiceMock
            .Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { role }.AsQueryable());

        roleServiceMock
            .Setup(expression: x => x.DeleteValidatedAsync(id: role.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(appId: 5);

        // Then
        roleServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        roleServiceMock.Verify(expression: x => x.DeleteValidatedAsync(id: role.Id), times: Times.Once);
        authorizationServiceMock.VerifyNoOtherCalls();
        privilegeServiceMock.VerifyNoOtherCalls();
    }
}