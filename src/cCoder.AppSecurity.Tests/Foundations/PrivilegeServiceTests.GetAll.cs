// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataPrivilege = cCoder.Data.Models.Security.Privilege;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class PrivilegeServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Privilege privilege = CreateRandomPrivilege();
        IQueryable<DataPrivilege> privileges = new[] { ToExternalPrivilege(privilege) }.AsQueryable();

        privilegeBrokerMock.Setup(x => x.GetAllPrivileges(false)).Returns(privileges);

        // When
        IQueryable<Privilege> result = privilegeService.GetAll();

        // Then
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(privilege);
        privilegeBrokerMock.Verify(x => x.GetAllPrivileges(false), Times.Once);
        privilegeBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Privilege>()), Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}