// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataUserRole = cCoder.Data.Models.Security.UserRole;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserRoleServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();
        IQueryable<DataUserRole> userRoles = new[] { ToExternalUserRole(item: userRole) }.AsQueryable();

        userRoleBrokerMock.Setup(expression: x => x.GetAllUserRoles(ignoreFilters: false))
            .Returns(value: userRoles);

        // When
        IQueryable<UserRole> result = userRoleService.GetAll();

        // Then
        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(expectation: userRole);

        userRoleBrokerMock.Verify(expression: x => x.GetAllUserRoles(ignoreFilters: false), times: Times.Once);
        userRoleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.UserRole>()), times: Times.AtMostOnce());
        userRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}