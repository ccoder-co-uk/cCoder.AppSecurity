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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        User entity = CreateRandomUser();
        userProcessingServiceMock.Setup(x => x.AddUserAsync(entity)).ReturnsAsync(entity);

        userEventProcessingServiceMock
            .Setup(x => x.RaiseUserAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        User result = await orchestrationService.AddUserAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        userProcessingServiceMock.Verify(x => x.AddUserAsync(entity), Times.Once);
        userEventProcessingServiceMock.Verify(x => x.RaiseUserAddEventAsync(entity), Times.Once);
    }

}