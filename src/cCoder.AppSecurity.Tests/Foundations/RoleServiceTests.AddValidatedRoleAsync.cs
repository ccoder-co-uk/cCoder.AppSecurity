// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using Role = cCoder.Data.Models.Security.Role;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public async Task ShouldAddValidatedRoleWithoutAuthorization()
    {
        // Given

        Role inputRole = CreateRandomRole();
        Role storedRole = ToExternalRole(item: inputRole);

        roleBrokerMock
            .Setup(expression: broker => broker.AddRoleAsync(
                entity: It.IsAny<Role>()))
            .ReturnsAsync(value: storedRole);

        // When

        Role actualRole = await roleService.AddValidatedRoleAsync(
            newRole: inputRole);

        // Then

        actualRole
            .Should()
            .BeSameAs(expected: inputRole);

        actualRole
            .Should()
            .BeEquivalentTo(expectation: storedRole);

        roleBrokerMock.Verify(
            expression: broker => broker.AddRoleAsync(entity: It.IsAny<Role>()),
            times: Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}