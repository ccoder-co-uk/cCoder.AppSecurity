// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        privilegeProcessingServiceMock.Setup(expression: x => x.Get(id: id)).Returns(value: entity);
        privilegeProcessingServiceMock.Setup(expression: x => x.DeleteAsync(id: id)).Returns(value: ValueTask.CompletedTask);

        privilegeEventProcessingServiceMock
            .Setup(expression: x => x.RaisePrivilegeDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(privilegeId: id);

        // Then
        privilegeProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        privilegeProcessingServiceMock.Verify(expression: x => x.DeleteAsync(id: id), times: Times.Once);
        privilegeEventProcessingServiceMock.Verify(expression: x => x.RaisePrivilegeDeleteEventAsync(entity: entity), times: Times.Once);
    }

}