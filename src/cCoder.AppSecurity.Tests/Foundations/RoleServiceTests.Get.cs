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

public partial class RoleServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        Role role = CreateRandomRole(id: roleId);

        roleBrokerMock.Setup(x => x.GetAllRoles(false)).Returns(new[] { ToExternalRole(role) }.AsQueryable());

        // When
        Role result = roleService.Get(roleId);

        // Then
        result.Should().BeEquivalentTo(
            role,
            options => options
                .Excluding(candidate => candidate.App)
                .Excluding(candidate => candidate.Pages)
                .Excluding(candidate => candidate.Folders)
                .Excluding(candidate => candidate.Users)
        );
        roleBrokerMock.Verify(x => x.GetAllRoles(false), Times.Once);
        roleBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.Role>()), Times.AtMostOnce());
        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}