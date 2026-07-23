// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class UserOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        User entity = CreateRandomUser();
        userProcessingServiceMock.Setup(x => x.UpdateUserAsync(entity)).ReturnsAsync(entity);

        userEventProcessingServiceMock
            .Setup(x => x.RaiseUserUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        User result = await orchestrationService.UpdateUserAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        userProcessingServiceMock.Verify(x => x.UpdateUserAsync(entity), Times.Once);
        userEventProcessingServiceMock.Verify(x => x.RaiseUserUpdateEventAsync(entity), Times.Once);
    }

}