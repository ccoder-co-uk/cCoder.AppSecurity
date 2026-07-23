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

        roleProcessingServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { entity }.AsQueryable());

        roleProcessingServiceMock.Setup(expression: x => x.DeleteAsync(id: id))
            .Returns(value: ValueTask.CompletedTask);

        roleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseRoleDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(roleId: id);

        // Then
        roleProcessingServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        roleProcessingServiceMock.Verify(expression: x => x.DeleteAsync(id: id), times: Times.Once);
        roleEventProcessingServiceMock.Verify(expression: x => x.RaiseRoleDeleteEventAsync(entity: entity), times: Times.Once);
    }

}