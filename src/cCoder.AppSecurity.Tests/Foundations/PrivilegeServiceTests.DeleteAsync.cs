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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege("page_read");

        privilegeBrokerMock.Setup(x => x.GetAllPrivileges(false)).Returns(new[] { ToExternalPrivilege(privilege) }.AsQueryable());

        privilegeBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Privilege_delete"));
        privilegeBrokerMock.Setup(x => x.DeletePrivilegeAsync(It.IsAny<cCoder.Data.Models.Security.Privilege>())).ReturnsAsync(1);

        // When
        await privilegeService.DeleteAsync("page_read");

        // Then
        privilegeBrokerMock.Verify(x => x.GetAllPrivileges(false), Times.Once);
        privilegeBrokerMock.Verify(x => x.DeletePrivilegeAsync(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.Once);
        privilegeBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Privilege_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege("page_read");

        privilegeBrokerMock.Setup(x => x.GetAllPrivileges(false)).Returns(new[] { ToExternalPrivilege(privilege) }.AsQueryable());

        privilegeBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Privilege_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await privilegeService.DeleteAsync("page_read");

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        privilegeBrokerMock.Verify(x => x.GetAllPrivileges(false), Times.Once);
        privilegeBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Privilege_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}