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

public partial class UserServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        User user = CreateRandomUser(id: "user-1");

        userBrokerMock.Setup(expression: x => x.GetAllUsers(ignoreFilters: false)).Returns(value: new[] { ToExternalUser(item: user) }.AsQueryable());

        userBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>())).Returns(value: (int?)7);
        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "User_delete"));
        userBrokerMock.Setup(expression: x => x.DeleteUserAsync(entity: It.IsAny<cCoder.Data.Models.Security.User>())).ReturnsAsync(value: 1);

        // When
        await userService.DeleteAsync(userId: "user-1");

        // Then
        userBrokerMock.Verify(expression: x => x.GetAllUsers(ignoreFilters: false), times: Times.Once);
        userBrokerMock.Verify(expression: x => x.DeleteUserAsync(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.Once);
        userBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "User_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        User user = CreateRandomUser(id: "user-1");

        userBrokerMock.Setup(expression: x => x.GetAllUsers(ignoreFilters: false)).Returns(value: new[] { ToExternalUser(item: user) }.AsQueryable());

        userBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>())).Returns(value: (int?)7);
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "User_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await userService.DeleteAsync(userId: "user-1");

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        userBrokerMock.Verify(expression: x => x.GetAllUsers(ignoreFilters: false), times: Times.Once);
        userBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "User_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}