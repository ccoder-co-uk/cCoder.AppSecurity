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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);

        IQueryable<cCoder.Data.Models.Security.Role> roles = new[]
        {
            ToExternalRole(item: role),
        }.AsQueryable();

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: true))
            .Returns(value: roles);

        roleBrokerMock
            .Setup(expression: broker => broker.GetAppId(
                entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: (int?)7,
                privilege: "Role_create"));

        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: CreateCurrentUser(
                appId: 7,
                privileges: [
                    "page_read",
                    "page_write",
                ]));

        roleBrokerMock
            .Setup(expression: broker => broker.AddRoleAsync(
                entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .ReturnsAsync(value: ToExternalRole(item: role));

        // When
        Role result = await roleService.AddRoleAsync(newRole: role);

        // Then
        Assert.Same(expected: role, actual: result);

        roleBrokerMock.Verify(
            expression: broker => broker.AddRoleAsync(
                entity: It.IsAny<cCoder.Data.Models.Security.Role>()),
            times: Times.Once);

        roleBrokerMock.Verify(
            expression: broker => broker.GetAllRoles(ignoreFilters: true),
            times: Times.Once);

        roleBrokerMock.Verify(
            expression: broker => broker.GetAppId(
                entity: It.IsAny<cCoder.Data.Models.Security.Role>()),
            times: Times.AtMostOnce());

        roleBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
            expression: broker => broker.Authorize(
                appId: (int?)7,
                privilege: "Role_create"),
            times: Times.Once);

        authorizationBrokerMock.Verify(
            expression: broker => broker.GetCurrentUser(),
            times: Times.Once);

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true))
            .Returns(value: new[] { ToExternalRole(item: role) }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await roleService.AddRoleAsync(newRole: role);

        // Then
        await action.Should()
            .ThrowAsync<cCoder.AppSecurity.Models.Exceptions.AppSecurityServiceException>()
            .WithMessage(expectedWildcardPattern: "The AppSecurity service failed.")
            .WithInnerException<cCoder.AppSecurity.Models.Exceptions.AppSecurityServiceException, SecurityException>(because: string.Empty, becauseArgs: [])
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Role_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAllowBootstrapForAddAsyncWhenAppHasNoExistingRoles()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);

        cCoder.Data.Models.Security.Role submitted = null;

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true))
            .Returns(value: Array.Empty<cCoder.Data.Models.Security.Role>()
            .AsQueryable());

        roleBrokerMock
            .Setup(expression: x => x.AddRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Callback<cCoder.Data.Models.Security.Role>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Security.Role value) => value);

        // When
        Role result = await roleService.AddRoleAsync(newRole: role);

        // Then
        result.Should()
            .BeSameAs(expected: role);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: role);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(
expectation: role,
config: options => options
                .Excluding(expression: candidate => candidate.App)
                .Excluding(expression: candidate => candidate.Pages)
                .Excluding(expression: candidate => candidate.Folders)
                .Excluding(expression: candidate => candidate.Users));

        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.AddRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}