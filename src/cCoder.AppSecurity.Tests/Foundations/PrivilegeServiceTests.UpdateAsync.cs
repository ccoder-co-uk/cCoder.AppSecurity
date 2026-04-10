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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege();

        cCoder.Data.Models.Security.Privilege submitted = null;

        privilegeBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Privilege_update"));

        privilegeBrokerMock
            .Setup(x => x.UpdatePrivilegeAsync(It.IsAny<cCoder.Data.Models.Security.Privilege>()))
            .Callback<cCoder.Data.Models.Security.Privilege>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Security.Privilege value) => value);

        // When
        Privilege result = await privilegeService.UpdateAsync(privilege);

        // Then
        result.Should().NotBeSameAs(privilege);
        submitted.Should().NotBeNull();
        submitted.Should().BeEquivalentTo(privilege);
        result.Should().BeEquivalentTo(privilege);
        privilegeBrokerMock.Verify(x => x.UpdatePrivilegeAsync(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.Once);
        privilegeBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Privilege_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege();

        privilegeBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Privilege_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await privilegeService.UpdateAsync(privilege);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        privilegeBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Privilege_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







