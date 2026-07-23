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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Role entity = CreateRandomRole();

        roleProcessingServiceMock.Setup(expression: x => x.AddRoleAsync(entity: entity))
            .ReturnsAsync(value: entity);

        roleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseRoleAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Role result = await orchestrationService.AddRoleAsync(newRole: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        roleProcessingServiceMock.Verify(expression: x => x.AddRoleAsync(entity: entity), times: Times.Once);
        roleEventProcessingServiceMock.Verify(expression: x => x.RaiseRoleAddEventAsync(entity: entity), times: Times.Once);
    }

    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddValidatedAsync()
    {
        // Given
        Role entity = CreateRandomRole();

        roleProcessingServiceMock.Setup(expression: x => x.AddValidatedRoleAsync(entity: entity))
            .ReturnsAsync(value: entity);

        roleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseRoleAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Role result = await orchestrationService.AddValidatedRoleAsync(newRole: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        roleProcessingServiceMock.Verify(expression: x => x.AddValidatedRoleAsync(entity: entity), times: Times.Once);
        roleEventProcessingServiceMock.Verify(expression: x => x.RaiseRoleAddEventAsync(entity: entity), times: Times.Once);
    }

}