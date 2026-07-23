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

public partial class PrivilegeServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege();

        cCoder.Data.Models.Security.Privilege submitted = null;

        privilegeBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Privilege_create"));

        privilegeBrokerMock
            .Setup(expression: x => x.AddPrivilegeAsync(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()))
            .Callback<cCoder.Data.Models.Security.Privilege>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Security.Privilege value) => value);

        // When
        Privilege result = await privilegeService.AddPrivilegeAsync(newPrivilege: privilege);

        // Then
        result.Should()
            .BeSameAs(expected: privilege);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: privilege);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: privilege, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: privilege, config: options => options.Excluding(expression: candidate => candidate.Id));

        privilegeBrokerMock.Verify(
expression: x => x.AddPrivilegeAsync(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()),
times: Times.Once
        );

        privilegeBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()), times: Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Privilege_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege();

        privilegeBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Privilege_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await privilegeService.AddPrivilegeAsync(newPrivilege: privilege);

        // Then
        await action.Should()
            .ThrowAsync<cCoder.AppSecurity.Models.Exceptions.AppSecurityServiceException>()
            .WithMessage(expectedWildcardPattern: "The AppSecurity service failed.")
            .WithInnerException<cCoder.AppSecurity.Models.Exceptions.AppSecurityServiceException, SecurityException>(because: string.Empty, becauseArgs: [])
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        privilegeBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()), times: Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Privilege_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}