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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        string id = Guid.NewGuid().ToString();
        User entity = CreateRandomUser();
        userProcessingServiceMock.Setup(expression: x => x.Get(id: id)).Returns(value: entity);
        userProcessingServiceMock.Setup(expression: x => x.DeleteAsync(id: id)).Returns(value: ValueTask.CompletedTask);

        userEventProcessingServiceMock
            .Setup(expression: x => x.RaiseUserDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(userId: id);

        // Then
        userProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        userProcessingServiceMock.Verify(expression: x => x.DeleteAsync(id: id), times: Times.Once);
        userEventProcessingServiceMock.Verify(expression: x => x.RaiseUserDeleteEventAsync(entity: entity), times: Times.Once);
    }

}