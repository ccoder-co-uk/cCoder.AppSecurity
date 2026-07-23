// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class RoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        Role entity = CreateRandomRole();
        entity.Id = id;
        roleProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { entity }.AsQueryable());
        roleProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        roleEventProcessingServiceMock
            .Setup(x => x.RaiseRoleDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        roleProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        roleProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        roleEventProcessingServiceMock.Verify(x => x.RaiseRoleDeleteEventAsync(entity), Times.Once);
    }

}