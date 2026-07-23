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

public partial class UserRoleServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        cCoder.Data.Models.Security.UserRole submitted = null;

        userRoleBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>())).Returns(value: (int?)7);
        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true)).Returns(value: new[]
        {
            CreateRole(roleId: userRole.RoleId, appId: 7, "user_read", "page_read")
        }.AsQueryable());
        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "UserRole_create"));
        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(value: CreateCurrentUser(appId: 7, "user_read", "page_read", "userrole_create"));

        userRoleBrokerMock
            .Setup(expression: x => x.AddUserRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>()))
            .Callback<cCoder.Data.Models.Security.UserRole>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Security.UserRole value) => value);

        // When
        UserRole result = await userRoleService.AddUserRoleAsync(newUserRole: userRole);

        // Then
        result.Should().BeSameAs(expected: userRole);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(unexpected: userRole);
        result.Should().NotBeSameAs(unexpected: submitted);
        submitted.Should().BeEquivalentTo(expectation: userRole);
        result.Should().BeEquivalentTo(expectation: userRole);
        userRoleBrokerMock.Verify(
expression:             x => x.AddUserRoleAsync(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>()),
times:             Times.Once
        );
        userRoleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>()), times: Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "UserRole_create"), times: Times.Once);
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserAssignsRoleWithUnownedPrivilegeForAddAsync()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        userRoleBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>())).Returns(value: (int?)7);
        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true)).Returns(value: new[]
        {
            CreateRole(roleId: userRole.RoleId, appId: 7, "app_admin", "page_read")
        }.AsQueryable());
        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "UserRole_create"));
        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(value: CreateCurrentUser(appId: 7, "userrole_create", "page_read"));

        // When
        Func<Task> action = async () => await userRoleService.AddUserRoleAsync(newUserRole: userRole);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        userRoleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>()), times: Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: true), times: Times.Once);
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "UserRole_create"), times: Times.Once);
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        userRoleBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>())).Returns(value: (int?)7);
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "UserRole_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await userRoleService.AddUserRoleAsync(newUserRole: userRole);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        userRoleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>()), times: Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "UserRole_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}