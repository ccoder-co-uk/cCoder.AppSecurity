// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public async Task ShouldReturnWhenDeleteRoleIsMissingAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        IQueryable<Role> roles = Array.Empty<Role>()
            .AsQueryable();

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: true))
            .Returns(value: roles);

        // When
        await roleService.DeleteAsync(roleId: roleId);

        // Then
        authorizationBrokerMock.VerifyNoOtherCalls();
        userRoleBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnWhenDeleteValidatedRoleIsMissingAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        IQueryable<Role> roles = Array.Empty<Role>()
            .AsQueryable();

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: true))
            .Returns(value: roles);

        // When
        await roleService.DeleteValidatedAsync(roleId: roleId);

        // Then
        authorizationBrokerMock.VerifyNoOtherCalls();
        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}