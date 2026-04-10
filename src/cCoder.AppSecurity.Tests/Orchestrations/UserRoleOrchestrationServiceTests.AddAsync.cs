using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class UserRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        UserRole entity = CreateRandomUserRole();
        userRoleProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        userRoleEventProcessingServiceMock
            .Setup(x => x.RaiseUserRoleAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        UserRole result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        userRoleProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        userRoleEventProcessingServiceMock.Verify(x => x.RaiseUserRoleAddEventAsync(entity), Times.Once);
    }

}







