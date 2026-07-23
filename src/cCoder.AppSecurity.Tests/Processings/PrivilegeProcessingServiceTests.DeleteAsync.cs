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


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class PrivilegeProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenUserHasDeletePrivilegeForDeleteAsync()
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

        const string privilegeId = "privilege_delete";
        User user = WithPrivilege(privilege: "privilege_delete");

        currentUser = user;

        // When
        Func<Task> act = async () => await privilegeProcessingService.DeleteAsync(privilegeId: privilegeId);

        // Then
        await act.Should()
            .ThrowAsync<cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingServiceException>()
            .WithMessage(expectedWildcardPattern: "The AppSecurity processing service failed.")
            .WithInnerException<cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingServiceException, InvalidOperationException>(because: string.Empty, becauseArgs: [])
            .WithMessage(expectedWildcardPattern: "Cannot delete privileges");

        privilegeServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
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

        const string privilegeId = "privilege_delete";
        User user = WithoutPrivileges();

        currentUser = user;

        // When
        Func<Task> act = async () => await privilegeProcessingService.DeleteAsync(privilegeId: privilegeId);

        // Then
        await act.Should()
            .ThrowAsync<cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingServiceException>()
            .WithMessage(expectedWildcardPattern: "The AppSecurity processing service failed.")
            .WithInnerException<cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingServiceException, SecurityException>(because: string.Empty, becauseArgs: [])
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        privilegeServiceMock.VerifyNoOtherCalls();
    }

}