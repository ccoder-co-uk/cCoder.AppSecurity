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
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Privilege[] entities = [CreateRandomPrivilege()];
        privilegeProcessingServiceMock.Setup(expression: x => x.DeleteAllPrivilegeAsync(items: entities)).Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllPrivilegeAsync(deletedPrivilege: entities);

        // Then
        privilegeProcessingServiceMock.Verify(expression: x => x.DeleteAllPrivilegeAsync(items: entities), times: Times.Once);
        privilegeProcessingServiceMock.VerifyNoOtherCalls();
        privilegeEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}