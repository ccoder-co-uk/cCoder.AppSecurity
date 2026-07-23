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
        Privilege privilege = CreateRandomPrivilege(id: "page_read");

        privilegeBrokerMock.Setup(expression: x => x.GetAllPrivileges(ignoreFilters: false)).Returns(value: new[] { ToExternalPrivilege(item: privilege) }.AsQueryable());

        privilegeBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>())).Returns(value: (int?)7);
        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Privilege_delete"));
        privilegeBrokerMock.Setup(expression: x => x.DeletePrivilegeAsync(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>())).ReturnsAsync(value: 1);

        // When
        await privilegeService.DeleteAsync(privilegeId: "page_read");

        // Then
        privilegeBrokerMock.Verify(expression: x => x.GetAllPrivileges(ignoreFilters: false), times: Times.Once);
        privilegeBrokerMock.Verify(expression: x => x.DeletePrivilegeAsync(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()), times: Times.Once);
        privilegeBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()), times: Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Privilege_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege(id: "page_read");

        privilegeBrokerMock.Setup(expression: x => x.GetAllPrivileges(ignoreFilters: false)).Returns(value: new[] { ToExternalPrivilege(item: privilege) }.AsQueryable());

        privilegeBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>())).Returns(value: (int?)7);
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Privilege_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await privilegeService.DeleteAsync(privilegeId: "page_read");

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        privilegeBrokerMock.Verify(expression: x => x.GetAllPrivileges(ignoreFilters: false), times: Times.Once);
        privilegeBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()), times: Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Privilege_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}