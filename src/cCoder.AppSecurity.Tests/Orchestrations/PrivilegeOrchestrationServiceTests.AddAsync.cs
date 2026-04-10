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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Privilege entity = CreateRandomPrivilege();
        privilegeProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        privilegeEventProcessingServiceMock
            .Setup(x => x.RaisePrivilegeAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Privilege result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        privilegeProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        privilegeEventProcessingServiceMock.Verify(x => x.RaisePrivilegeAddEventAsync(entity), Times.Once);
    }

}







