// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {

                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }

            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => ToExternalUser(user: currentUser));


        // When
        Privilege privilege = new()
        {
            Id = "privilege_read",
            Type = "Privilege",
            Operation = "Read",
            Description = "Read privileges",
        };

        privilegeServiceMock.Setup(expression: x => x.Get(id: privilege.Id))
            .Returns(value: privilege);

        currentUser = WithPrivilege(privilege: "privilege_read");
        Privilege result = privilegeProcessingService.Get(privilegeId: privilege.Id);

        // Then
        Assert.Same(expected: privilege, actual: result);
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenUserLacksReadPrivilegeForGet()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {

                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }

            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => ToExternalUser(user: currentUser));


        // When
        currentUser = WithoutPrivileges();

        // Then
        cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingServiceException exception =
            Assert.Throws<cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingServiceException>(
                testCode: () => privilegeProcessingService.Get(
                    privilegeId: "privilege_read"));

        Assert.IsType<SecurityException>(
            @object: exception.InnerException);
    }

}