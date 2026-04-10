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
        roleProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        roleEventProcessingServiceMock
            .Setup(x => x.RaiseRoleAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Role result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        roleProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        roleEventProcessingServiceMock.Verify(x => x.RaiseRoleAddEventAsync(entity), Times.Once);
    }

    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddValidatedAsync()
    {
        // Given
        Role entity = CreateRandomRole();
        roleProcessingServiceMock.Setup(x => x.AddValidatedAsync(entity)).ReturnsAsync(entity);

        roleEventProcessingServiceMock
            .Setup(x => x.RaiseRoleAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Role result = await orchestrationService.AddValidatedAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        roleProcessingServiceMock.Verify(x => x.AddValidatedAsync(entity), Times.Once);
        roleEventProcessingServiceMock.Verify(x => x.RaiseRoleAddEventAsync(entity), Times.Once);
    }

}







