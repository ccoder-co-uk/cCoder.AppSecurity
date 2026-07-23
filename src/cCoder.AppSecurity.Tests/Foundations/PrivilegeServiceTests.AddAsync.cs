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

        privilegeBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Privilege_create"));

        privilegeBrokerMock
            .Setup(x => x.AddPrivilegeAsync(It.IsAny<cCoder.Data.Models.Security.Privilege>()))
            .Callback<cCoder.Data.Models.Security.Privilege>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Security.Privilege value) => value);

        // When
        Privilege result = await privilegeService.AddAsync(privilege);

        // Then
        result.Should().BeSameAs(privilege);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(privilege);
        result.Should().NotBeSameAs(submitted);

        submitted
            .Should()
            .BeEquivalentTo(privilege, options => options.Excluding(candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(privilege, options => options.Excluding(candidate => candidate.Id));

        privilegeBrokerMock.Verify(
            x => x.AddPrivilegeAsync(It.IsAny<cCoder.Data.Models.Security.Privilege>()),
            Times.Once
        );
        privilegeBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Privilege_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege();

        privilegeBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Privilege_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await privilegeService.AddAsync(privilege);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        privilegeBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Privilege_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}