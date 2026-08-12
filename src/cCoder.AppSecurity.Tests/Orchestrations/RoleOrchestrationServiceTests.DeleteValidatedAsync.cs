// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class RoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventThenDeleteWhenDeleteValidatedAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        Role role = CreateRandomRole();
        role.Id = roleId;

        roleProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { role }.AsQueryable());

        roleEventProcessingServiceMock
            .Setup(expression: service => service.RaiseRoleDeleteEventAsync(
                entity: role))
            .Returns(value: ValueTask.CompletedTask);

        roleProcessingServiceMock
            .Setup(expression: service => service.DeleteValidatedAsync(id: roleId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteValidatedAsync(roleId: roleId);

        // Then
        roleEventProcessingServiceMock.Verify(
            expression: service => service.RaiseRoleDeleteEventAsync(entity: role),
            times: Times.Once);

        roleProcessingServiceMock.Verify(
            expression: service => service.DeleteValidatedAsync(id: roleId),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldReturnWithoutDeleteWhenDeleteValidatedRoleIsMissingAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        IQueryable<Role> roles = Array.Empty<Role>()
            .AsQueryable();

        roleProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: roles);

        // When
        await orchestrationService.DeleteValidatedAsync(roleId: roleId);

        // Then
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}