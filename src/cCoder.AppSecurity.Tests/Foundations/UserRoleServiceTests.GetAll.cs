using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataUserRole = cCoder.Data.Models.Security.UserRole;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserRoleServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();
        IQueryable<DataUserRole> userRoles = new[] { ToExternalUserRole(userRole) }.AsQueryable();

        userRoleBrokerMock.Setup(x => x.GetAllUserRoles(false)).Returns(userRoles);

        // When
        IQueryable<UserRole> result = userRoleService.GetAll();

        // Then
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(userRole);
        userRoleBrokerMock.Verify(x => x.GetAllUserRoles(false), Times.Once);
        userRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.UserRole>()), Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}








