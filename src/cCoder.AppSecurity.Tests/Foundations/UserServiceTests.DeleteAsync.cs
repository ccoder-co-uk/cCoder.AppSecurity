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
        User user = CreateRandomUser("user-1");

        userBrokerMock.Setup(x => x.GetAllUsers(false)).Returns(new[] { ToExternalUser(user) }.AsQueryable());

        userBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "User_delete"));
        userBrokerMock.Setup(x => x.DeleteUserAsync(It.IsAny<cCoder.Data.Models.Security.User>())).ReturnsAsync(1);

        // When
        await userService.DeleteAsync("user-1");

        // Then
        userBrokerMock.Verify(x => x.GetAllUsers(false), Times.Once);
        userBrokerMock.Verify(x => x.DeleteUserAsync(It.IsAny<cCoder.Data.Models.Security.User>()), Times.Once);
        userBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>()), Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "User_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        User user = CreateRandomUser("user-1");

        userBrokerMock.Setup(x => x.GetAllUsers(false)).Returns(new[] { ToExternalUser(user) }.AsQueryable());

        userBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "User_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await userService.DeleteAsync("user-1");

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        userBrokerMock.Verify(x => x.GetAllUsers(false), Times.Once);
        userBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>()), Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "User_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}