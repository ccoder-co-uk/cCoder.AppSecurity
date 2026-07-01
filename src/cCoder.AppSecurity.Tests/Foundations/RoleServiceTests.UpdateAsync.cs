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

        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[] { ToExternalRole(role) }.AsQueryable());
        roleBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Role_update"));
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(CreateCurrentUser(7, "page_read", "page_write"));

        roleBrokerMock
            .Setup(x => x.UpdateRoleAsync(It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Callback<cCoder.Data.Models.Security.Role>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Security.Role value) => value);

        // When
        Role result = await roleService.UpdateAsync(role);

        // Then
        result.Should().BeSameAs(role);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(role);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(
            role,
            options => options
                .Excluding(candidate => candidate.App)
                .Excluding(candidate => candidate.Pages)
                .Excluding(candidate => candidate.Folders)
                .Excluding(candidate => candidate.Users)
        );
        result.Should().BeEquivalentTo(
            role,
            options => options
                .Excluding(candidate => candidate.App)
                .Excluding(candidate => candidate.Pages)
                .Excluding(candidate => candidate.Folders)
                .Excluding(candidate => candidate.Users)
        );
        roleBrokerMock.Verify(x => x.GetAllRoles(true), Times.Once);
        roleBrokerMock.Verify(x => x.UpdateRoleAsync(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.Once);
        roleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Role_update"), Times.Once);
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserAssignsUnownedPrivilegeForUpdateAsync()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);
        role.Privs = "page_read,page_delete";

        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[] { ToExternalRole(role) }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Role_update"));
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(CreateCurrentUser(7, "page_read", "role_update"));

        // When
        Func<Task> action = async () => await roleService.UpdateAsync(role);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        roleBrokerMock.Verify(x => x.GetAllRoles(true), Times.Once);
        roleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Role_update"), Times.Once);
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);

        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(new[] { ToExternalRole(role) }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Role_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await roleService.UpdateAsync(role);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        roleBrokerMock.Verify(x => x.GetAllRoles(true), Times.Once);
        roleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Role_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAllowBootstrapForUpdateAsyncWhenAppHasNoExistingRoles()
    {
        // Given
        Role role = CreateRandomRole(appId: 7);

        cCoder.Data.Models.Security.Role submitted = null;

        roleBrokerMock.Setup(x => x.GetAllRoles(true)).Returns(Array.Empty<cCoder.Data.Models.Security.Role>().AsQueryable());
        roleBrokerMock
            .Setup(x => x.UpdateRoleAsync(It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Callback<cCoder.Data.Models.Security.Role>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Security.Role value) => value);

        // When
        Role result = await roleService.UpdateAsync(role);

        // Then
        result.Should().BeSameAs(role);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(role);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(
            role,
            options => options
                .Excluding(candidate => candidate.App)
                .Excluding(candidate => candidate.Pages)
                .Excluding(candidate => candidate.Folders)
                .Excluding(candidate => candidate.Users));
        roleBrokerMock.Verify(x => x.GetAllRoles(true), Times.Once);
        roleBrokerMock.Verify(x => x.UpdateRoleAsync(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.Once);
        roleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







