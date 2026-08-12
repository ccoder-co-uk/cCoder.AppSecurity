// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Moq;
using Xunit;
using Role = cCoder.Data.Models.Security.Role;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public async Task ShouldReturnWithoutDeletingValidatedMissingRole()
    {
        // Given

        Guid roleId = Guid.NewGuid();

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: true))
            .Returns(value: Array.Empty<Role>()
                .AsQueryable());

        // When

        await roleService.DeleteValidatedAsync(roleId: roleId);

        // Then

        roleBrokerMock.Verify(
            expression: broker => broker.GetAllRoles(ignoreFilters: true),
            times: Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}