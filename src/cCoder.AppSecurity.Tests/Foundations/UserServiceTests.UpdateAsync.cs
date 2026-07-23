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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        User user = CreateRandomUser();

        cCoder.Data.Models.Security.User submitted = null;

        userBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "User_update"));

        userBrokerMock
            .Setup(x => x.UpdateUserAsync(It.IsAny<cCoder.Data.Models.Security.User>()))
            .Callback<cCoder.Data.Models.Security.User>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Security.User value) => value);

        // When
        User result = await userService.UpdateUserAsync(user);

        // Then
        result.Should().BeSameAs(user);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(user);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(
            user,
            options => options
                .Excluding(candidate => candidate.DefaultCulture)
                .Excluding(candidate => candidate.Roles));
        result.Should().BeEquivalentTo(user);
        userBrokerMock.Verify(x => x.UpdateUserAsync(It.IsAny<cCoder.Data.Models.Security.User>()), Times.Once);
        userBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>()), Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "User_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        User user = CreateRandomUser();

        userBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "User_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await userService.UpdateUserAsync(user);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        userBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>()), Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "User_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}