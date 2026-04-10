using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class PrivilegeOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        string id = Guid.NewGuid().ToString();
        Privilege entity = CreateRandomPrivilege();
        privilegeProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        privilegeProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        privilegeEventProcessingServiceMock
            .Setup(x => x.RaisePrivilegeDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        privilegeProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        privilegeProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        privilegeEventProcessingServiceMock.Verify(x => x.RaisePrivilegeDeleteEventAsync(entity), Times.Once);
    }

}







