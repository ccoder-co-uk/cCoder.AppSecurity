// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        Role role = CreateRandomRole(id: roleId, appId: 7);

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true))
            .Returns(value: new[] { ToExternalRole(item: role) }.AsQueryable());

        userRoleBrokerMock.Setup(expression: x => x.GetAllUserRoles(ignoreFilters: true))
            .Returns(value: Array.Empty<UserRole>()
            .AsQueryable());

        roleBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_delete"));

        roleBrokerMock.Setup(expression: x => x.DeletePageRolesByRoleIdAsync(roleId: roleId))
            .Returns(value: ValueTask.CompletedTask);

        roleBrokerMock.Setup(expression: x => x.DeleteFolderRolesByRoleIdAsync(roleId: roleId))
            .Returns(value: ValueTask.CompletedTask);

        roleBrokerMock.Setup(expression: x => x.DeleteRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .ReturnsAsync(value: 1);

        // When
        await roleService.DeleteAsync(roleId: roleId);

        // Then
        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.DeletePageRolesByRoleIdAsync(roleId: roleId), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.DeleteFolderRolesByRoleIdAsync(roleId: roleId), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.DeleteRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        userRoleBrokerMock.Verify(expression: x => x.GetAllUserRoles(ignoreFilters: true), times: Times.Once);
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        Role role = CreateRandomRole(id: roleId, appId: 7);

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true))
            .Returns(value: new[] { ToExternalRole(item: role) }.AsQueryable());

        userRoleBrokerMock.Setup(expression: x => x.GetAllUserRoles(ignoreFilters: true))
            .Returns(value: Array.Empty<UserRole>()
            .AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await roleService.DeleteAsync(roleId: roleId);

        // Then
        await action.Should()
            .ThrowAsync<cCoder.AppSecurity.Models.Exceptions.AppSecurityServiceException>()
            .WithMessage(expectedWildcardPattern: "The AppSecurity service failed.")
            .WithInnerException<cCoder.AppSecurity.Models.Exceptions.AppSecurityServiceException, SecurityException>(because: string.Empty, becauseArgs: [])
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}