using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class PrivilegeProcessingServiceTests
{
    [Fact]
    public void ShouldReturnRequestedPrivilegeWhenUserHasReadPrivilegeForGet()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));


        // When
        Privilege privilege = new()
        {
            Id = "privilege_read",
            Type = "Privilege",
            Operation = "Read",
            Description = "Read privileges",
        };
        privilegeServiceMock.Setup(x => x.Get(privilege.Id)).Returns(privilege);
        currentUser = WithPrivilege("privilege_read");
        Privilege result = privilegeProcessingService.Get(privilege.Id);

        // Then
        Assert.Same(privilege, result);
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenUserLacksReadPrivilegeForGet()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));


        // When
        currentUser = WithoutPrivileges();

        // Then
        Assert.Throws<SecurityException>(() => privilegeProcessingService.Get("privilege_read"));
    }

}







