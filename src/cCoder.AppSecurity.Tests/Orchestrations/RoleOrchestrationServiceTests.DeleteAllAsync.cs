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
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Role[] entities = [CreateRandomRole()];
        roleProcessingServiceMock.Setup(expression: x => x.DeleteAllRoleAsync(items: entities)).Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllRoleAsync(deletedRole: entities);

        // Then
        roleProcessingServiceMock.Verify(expression: x => x.DeleteAllRoleAsync(items: entities), times: Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}