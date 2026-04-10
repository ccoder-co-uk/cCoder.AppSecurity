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

        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[] { ToExternalRole(role) }.AsQueryable());
        userRoleBrokerMock.Setup(x => x.GetAllUserRoles(true)).Returns(Array.Empty<UserRole>().AsQueryable());

        roleBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Role_delete"));
        roleBrokerMock.Setup(x => x.DeleteRoleAsync(It.IsAny<cCoder.Data.Models.Security.Role>())).ReturnsAsync(1);

        // When
        await roleService.DeleteAsync(roleId);

        // Then
        roleBrokerMock.Verify(x => x.GetAllRoles(true), Times.Once);
        roleBrokerMock.Verify(x => x.DeleteRoleAsync(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.Once);
        roleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        userRoleBrokerMock.Verify(x => x.GetAllUserRoles(true), Times.Once);
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Role_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        Role role = CreateRandomRole(id: roleId, appId: 7);

        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[] { ToExternalRole(role) }.AsQueryable());
        userRoleBrokerMock.Setup(x => x.GetAllUserRoles(true)).Returns(Array.Empty<UserRole>().AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Role_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await roleService.DeleteAsync(roleId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        roleBrokerMock.Verify(x => x.GetAllRoles(true), Times.Once);
        roleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Role_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







