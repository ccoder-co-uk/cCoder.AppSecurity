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
        IQueryable<DataPrivilege> privileges = new[] { ToExternalPrivilege(item: privilege) }.AsQueryable();

        privilegeBrokerMock.Setup(expression: x => x.GetAllPrivileges(ignoreFilters: false))
            .Returns(value: privileges);

        // When
        IQueryable<Privilege> result = privilegeService.GetAll();

        // Then
        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(expectation: privilege);

        privilegeBrokerMock.Verify(expression: x => x.GetAllPrivileges(ignoreFilters: false), times: Times.Once);
        privilegeBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()), times: Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}