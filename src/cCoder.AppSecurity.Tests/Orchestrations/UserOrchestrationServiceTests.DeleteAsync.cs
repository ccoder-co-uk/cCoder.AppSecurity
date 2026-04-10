using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class UserOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        string id = Guid.NewGuid().ToString();
        User entity = CreateRandomUser();
        userProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        userProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        userEventProcessingServiceMock
            .Setup(x => x.RaiseUserDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        userProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        userProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        userEventProcessingServiceMock.Verify(x => x.RaiseUserDeleteEventAsync(entity), Times.Once);
    }

}







