using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class PrivilegeOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Privilege[] entities = [CreateRandomPrivilege()];
        privilegeProcessingServiceMock.Setup(x => x.DeleteAllAsync(entities)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        privilegeProcessingServiceMock.Verify(x => x.DeleteAllAsync(entities), Times.Once);
        privilegeProcessingServiceMock.VerifyNoOtherCalls();
        privilegeEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







