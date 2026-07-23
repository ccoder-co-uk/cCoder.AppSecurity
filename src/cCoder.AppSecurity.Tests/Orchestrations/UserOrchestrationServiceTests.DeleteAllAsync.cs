// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class UserOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        User[] entities = [CreateRandomUser()];
        userProcessingServiceMock.Setup(expression: x => x.DeleteAllUserAsync(items: entities)).Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllUserAsync(deletedUser: entities);

        // Then
        userProcessingServiceMock.Verify(expression: x => x.DeleteAllUserAsync(items: entities), times: Times.Once);
        userProcessingServiceMock.VerifyNoOtherCalls();
        userEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}