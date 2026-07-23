// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class RoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Role entity = CreateRandomRole();

        roleProcessingServiceMock.Setup(expression: x => x.UpdateRoleAsync(entity: entity))
            .ReturnsAsync(value: entity);

        roleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseRoleUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Role result = await orchestrationService.UpdateRoleAsync(updatedRole: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        roleProcessingServiceMock.Verify(expression: x => x.UpdateRoleAsync(entity: entity), times: Times.Once);
        roleEventProcessingServiceMock.Verify(expression: x => x.RaiseRoleUpdateEventAsync(entity: entity), times: Times.Once);
    }

    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateValidatedAsync()
    {
        // Given
        Role entity = CreateRandomRole();

        roleProcessingServiceMock.Setup(expression: x => x.UpdateValidatedRoleAsync(entity: entity))
            .ReturnsAsync(value: entity);

        roleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseRoleUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Role result = await orchestrationService.UpdateValidatedRoleAsync(updatedRole: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        roleProcessingServiceMock.Verify(expression: x => x.UpdateValidatedRoleAsync(entity: entity), times: Times.Once);
        roleEventProcessingServiceMock.Verify(expression: x => x.RaiseRoleUpdateEventAsync(entity: entity), times: Times.Once);
    }

}