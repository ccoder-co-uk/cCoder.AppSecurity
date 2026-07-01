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

        userRoleBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>())).Returns((int?)7);
        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[]
        {
            CreateRole(userRole.RoleId, 7, "user_read", "page_read")
        }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "UserRole_create"));
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(CreateCurrentUser(7, "user_read", "page_read", "userrole_create"));

        userRoleBrokerMock
            .Setup(x => x.AddUserRoleAsync(It.IsAny<cCoder.Data.Models.Security.UserRole>()))
            .Callback<cCoder.Data.Models.Security.UserRole>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Security.UserRole value) => value);

        // When
        UserRole result = await userRoleService.AddAsync(userRole);

        // Then
        result.Should().BeSameAs(userRole);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(userRole);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(userRole);
        result.Should().BeEquivalentTo(userRole);
        userRoleBrokerMock.Verify(
            x => x.AddUserRoleAsync(It.IsAny<cCoder.Data.Models.Security.UserRole>()),
            Times.Once
        );
        userRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>()), Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        roleBrokerMock.Verify(x => x.GetAllRoles(true), Times.Once);
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "UserRole_create"), Times.Once);
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserAssignsRoleWithUnownedPrivilegeForAddAsync()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        userRoleBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>())).Returns((int?)7);
        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[]
        {
            CreateRole(userRole.RoleId, 7, "app_admin", "page_read")
        }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "UserRole_create"));
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(CreateCurrentUser(7, "userrole_create", "page_read"));

        // When
        Func<Task> action = async () => await userRoleService.AddAsync(userRole);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        userRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>()), Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        roleBrokerMock.Verify(x => x.GetAllRoles(true), Times.Once);
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "UserRole_create"), Times.Once);
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        userRoleBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "UserRole_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await userRoleService.AddAsync(userRole);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        userRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>()), Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "UserRole_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







