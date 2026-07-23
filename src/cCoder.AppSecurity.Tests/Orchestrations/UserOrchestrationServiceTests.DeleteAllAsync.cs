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
        userProcessingServiceMock.Setup(x => x.DeleteAllAsync(entities)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        userProcessingServiceMock.Verify(x => x.DeleteAllAsync(entities), Times.Once);
        userProcessingServiceMock.VerifyNoOtherCalls();
        userEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}