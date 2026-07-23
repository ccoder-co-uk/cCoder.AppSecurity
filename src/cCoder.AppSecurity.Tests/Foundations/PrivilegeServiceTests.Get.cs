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
        Privilege privilege = CreateRandomPrivilege(id: "page_read");

        privilegeBrokerMock.Setup(expression: x => x.GetAllPrivileges(ignoreFilters: false))
            .Returns(value: new[] { ToExternalPrivilege(item: privilege) }.AsQueryable());

        // When
        Privilege result = privilegeService.Get(privilegeId: "page_read");

        // Then
        result.Should()
            .BeEquivalentTo(expectation: privilege);

        privilegeBrokerMock.Verify(expression: x => x.GetAllPrivileges(ignoreFilters: false), times: Times.Once);
        privilegeBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Privilege>()), times: Times.AtMostOnce());
        privilegeBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}