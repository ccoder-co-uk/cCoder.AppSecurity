// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege("page_read");

        privilegeBrokerMock.Setup(x => x.GetAllPrivileges(false)).Returns(new[] { ToExternalPrivilege(privilege) }.AsQueryable());

        // When
        Privilege result = privilegeService.Get("page_read");

        // Then
        result.Should().BeEquivalentTo(privilege);
        privilegeBrokerMock.Verify(x => x.GetAllPrivileges(false), Times.Once);
        privilegeBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}