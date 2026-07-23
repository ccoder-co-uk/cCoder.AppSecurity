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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);

        cCoder.Data.Models.Security.Role submitted = null;

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true)).Returns(value: new[] { ToExternalRole(item: role) }.AsQueryable());
        roleBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>())).Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_update"));
        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(value: CreateCurrentUser(appId: 7, "page_read", "page_write"));

        roleBrokerMock
            .Setup(expression: x => x.UpdateRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Callback<cCoder.Data.Models.Security.Role>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Security.Role value) => value);

        // When
        Role result = await roleService.UpdateRoleAsync(updatedRole: role);

        // Then
        result.Should().BeSameAs(expected: role);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(unexpected: role);
        result.Should().NotBeSameAs(unexpected: submitted);
        submitted.Should().BeEquivalentTo(
expectation:             role,
config:             options => options
                .Excluding(expression: candidate => candidate.App)
                .Excluding(expression: candidate => candidate.Pages)
                .Excluding(expression: candidate => candidate.Folders)
                .Excluding(expression: candidate => candidate.Users)
        );
        result.Should().BeEquivalentTo(
expectation:             role,
config:             options => options
                .Excluding(expression: candidate => candidate.App)
                .Excluding(expression: candidate => candidate.Pages)
                .Excluding(expression: candidate => candidate.Folders)
                .Excluding(expression: candidate => candidate.Users)
        );
        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.UpdateRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_update"), times: Times.Once);
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserAssignsUnownedPrivilegeForUpdateAsync()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);
        role.Privs = "page_read,page_delete";

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true)).Returns(value: new[] { ToExternalRole(item: role) }.AsQueryable());
        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_update"));
        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(value: CreateCurrentUser(appId: 7, "page_read", "role_update"));

        // When
        Func<Task> action = async () => await roleService.UpdateRoleAsync(updatedRole: role);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_update"), times: Times.Once);
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true)).Returns(value: new[] { ToExternalRole(item: role) }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await roleService.UpdateRoleAsync(updatedRole: role);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAllowBootstrapForUpdateAsyncWhenAppHasNoExistingRoles()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);

        cCoder.Data.Models.Security.Role submitted = null;

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true)).Returns(value: Array.Empty<cCoder.Data.Models.Security.Role>().AsQueryable());
        roleBrokerMock
            .Setup(expression: x => x.UpdateRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Callback<cCoder.Data.Models.Security.Role>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Security.Role value) => value);

        // When
        Role result = await roleService.UpdateRoleAsync(updatedRole: role);

        // Then
        result.Should().BeSameAs(expected: role);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(unexpected: role);
        result.Should().NotBeSameAs(unexpected: submitted);
        submitted.Should().BeEquivalentTo(
expectation:             role,
config:             options => options
                .Excluding(expression: candidate => candidate.App)
                .Excluding(expression: candidate => candidate.Pages)
                .Excluding(expression: candidate => candidate.Folders)
                .Excluding(expression: candidate => candidate.Users));
        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.UpdateRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}