using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class RoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Role[] entities = [CreateRandomRole()];
        roleProcessingServiceMock.Setup(x => x.DeleteAllAsync(entities)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        roleProcessingServiceMock.Verify(x => x.DeleteAllAsync(entities), Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







