// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataRole = cCoder.Data.Models.Security.Role;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Role role = CreateRandomRole();
        IQueryable<DataRole> roles = new[] { ToExternalRole(item: role) }.AsQueryable();

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: false))
            .Returns(value: roles);

        // When
        IQueryable<Role> result = roleService.GetAll();

        // Then
        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
expectation: role,
config: options => options
                .Excluding(expression: candidate => candidate.App)
                .Excluding(expression: candidate => candidate.Pages)
                .Excluding(expression: candidate => candidate.Folders)
                .Excluding(expression: candidate => candidate.Users)
        );

        roleBrokerMock.Verify(expression: x => x.GetAllRoles(ignoreFilters: false), times: Times.Once);
        roleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.Role>()), times: Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}