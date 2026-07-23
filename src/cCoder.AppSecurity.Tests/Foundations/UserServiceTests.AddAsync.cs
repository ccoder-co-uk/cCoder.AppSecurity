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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        User user = CreateRandomUser();

        cCoder.Data.Models.Security.User submitted = null;

        userBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>())).Returns(value: (int?)7);
        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "User_create"));

        userBrokerMock
            .Setup(expression: x => x.AddUserAsync(entity: It.IsAny<cCoder.Data.Models.Security.User>()))
            .Callback<cCoder.Data.Models.Security.User>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Security.User value) => value);

        // When
        User result = await userService.AddUserAsync(newUser: user);

        // Then
        result.Should().BeSameAs(expected: user);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(unexpected: user);
        result.Should().NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation:                 user,
config:                 options => options
                    .Excluding(expression: candidate => candidate.DefaultCulture)
                    .Excluding(expression: candidate => candidate.Roles));

        result
            .Should()
            .BeEquivalentTo(expectation: user);

        userBrokerMock.Verify(
expression:             x => x.AddUserAsync(entity: It.IsAny<cCoder.Data.Models.Security.User>()),
times:             Times.Once
        );
        userBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "User_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        User user = CreateRandomUser();

        userBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>())).Returns(value: (int?)7);
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "User_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await userService.AddUserAsync(newUser: user);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        userBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "User_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}