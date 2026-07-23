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

        userProcessingServiceMock.Setup(expression: x => x.AddUserAsync(entity: entity))
            .ReturnsAsync(value: entity);

        userEventProcessingServiceMock
            .Setup(expression: x => x.RaiseUserAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        User result = await orchestrationService.AddUserAsync(newUser: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        userProcessingServiceMock.Verify(expression: x => x.AddUserAsync(entity: entity), times: Times.Once);
        userEventProcessingServiceMock.Verify(expression: x => x.RaiseUserAddEventAsync(entity: entity), times: Times.Once);
    }

}