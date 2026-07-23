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
        roleProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        roleEventProcessingServiceMock
            .Setup(x => x.RaiseRoleUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Role result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        roleProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        roleEventProcessingServiceMock.Verify(x => x.RaiseRoleUpdateEventAsync(entity), Times.Once);
    }

    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateValidatedAsync()
    {
        // Given
        Role entity = CreateRandomRole();
        roleProcessingServiceMock.Setup(x => x.UpdateValidatedAsync(entity)).ReturnsAsync(entity);

        roleEventProcessingServiceMock
            .Setup(x => x.RaiseRoleUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Role result = await orchestrationService.UpdateValidatedAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        roleProcessingServiceMock.Verify(x => x.UpdateValidatedAsync(entity), Times.Once);
        roleEventProcessingServiceMock.Verify(x => x.RaiseRoleUpdateEventAsync(entity), Times.Once);
    }

}