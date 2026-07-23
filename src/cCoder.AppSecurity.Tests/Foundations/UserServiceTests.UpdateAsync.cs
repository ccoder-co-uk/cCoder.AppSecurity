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

        userBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "User_update"));

        userBrokerMock
            .Setup(expression: x => x.UpdateUserAsync(entity: It.IsAny<cCoder.Data.Models.Security.User>()))
            .Callback<cCoder.Data.Models.Security.User>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Security.User value) => value);

        // When
        User result = await userService.UpdateUserAsync(updatedUser: user);

        // Then
        result.Should()
            .BeSameAs(expected: user);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: user);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(
expectation: user,
config: options => options
                .Excluding(expression: candidate => candidate.DefaultCulture)
                .Excluding(expression: candidate => candidate.Roles));

        result.Should()
            .BeEquivalentTo(expectation: user);

        userBrokerMock.Verify(expression: x => x.UpdateUserAsync(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.Once);
        userBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "User_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        User user = CreateRandomUser();

        userBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "User_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await userService.UpdateUserAsync(updatedUser: user);

        // Then
        await action.Should()
            .ThrowAsync<cCoder.AppSecurity.Models.Exceptions.AppSecurityServiceException>()
            .WithMessage(expectedWildcardPattern: "The AppSecurity service failed.")
            .WithInnerException<cCoder.AppSecurity.Models.Exceptions.AppSecurityServiceException, SecurityException>(because: string.Empty, becauseArgs: [])
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        userBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "User_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}