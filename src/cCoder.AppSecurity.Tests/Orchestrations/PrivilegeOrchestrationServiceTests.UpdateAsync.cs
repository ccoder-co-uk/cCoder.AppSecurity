using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class PrivilegeOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Privilege entity = CreateRandomPrivilege();
        privilegeProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        privilegeEventProcessingServiceMock
            .Setup(x => x.RaisePrivilegeUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Privilege result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        privilegeProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        privilegeEventProcessingServiceMock.Verify(x => x.RaisePrivilegeUpdateEventAsync(entity), Times.Once);
    }

}







