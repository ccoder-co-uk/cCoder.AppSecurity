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
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new cCoder.Data.Models.Security.User { Id = "admin" });
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "UserRole_create"));

        userRoleBrokerMock
            .Setup(x => x.AddUserRoleAsync(It.IsAny<cCoder.Data.Models.Security.UserRole>()))
            .Callback<cCoder.Data.Models.Security.UserRole>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Security.UserRole value) => value);

        // When
        UserRole result = await userRoleService.AddAsync(userRole);

        // Then
        result.Should().NotBeSameAs(userRole);
        submitted.Should().NotBeNull();
        submitted.Should().BeEquivalentTo(userRole);
        result.Should().BeEquivalentTo(userRole);
        userRoleBrokerMock.Verify(
            x => x.AddUserRoleAsync(It.IsAny<cCoder.Data.Models.Security.UserRole>()),
            Times.Once
        );
        userRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>()), Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "UserRole_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        userRoleBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new cCoder.Data.Models.Security.User { Id = "admin" });
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "UserRole_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await userRoleService.AddAsync(userRole);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        userRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>()), Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "UserRole_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAllowBootstrapForAddAsyncWhenAppHasNoNonGuestUsers()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        cCoder.Data.Models.Security.UserRole submitted = null;

        authorizationBrokerMock
            .Setup(x => x.GetCurrentUser())
            .Returns(new cCoder.Data.Models.Security.User { Id = "Guest" });

        userRoleBrokerMock
            .Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>()))
            .Returns((int?)7);

        userRoleBrokerMock
            .Setup(x => x.GetAllUserRoles(true))
            .Returns(new[]
            {
                new cCoder.Data.Models.Security.UserRole
                {
                    RoleId = Guid.NewGuid(),
                    UserId = "Guest"
                }
            }.AsQueryable());

        userRoleBrokerMock
            .Setup(x => x.AddUserRoleAsync(It.IsAny<cCoder.Data.Models.Security.UserRole>()))
            .Callback<cCoder.Data.Models.Security.UserRole>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Security.UserRole value) => value);

        // When
        UserRole result = await userRoleService.AddAsync(userRole);

        // Then
        result.Should().NotBeSameAs(userRole);
        submitted.Should().NotBeNull();
        submitted.Should().BeEquivalentTo(userRole);
        result.Should().BeEquivalentTo(userRole);

        userRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>()), Times.Once);
        userRoleBrokerMock.Verify(x => x.GetAllUserRoles(true), Times.Once);
        userRoleBrokerMock.Verify(
            x => x.AddUserRoleAsync(It.IsAny<cCoder.Data.Models.Security.UserRole>()),
            Times.Once);
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







