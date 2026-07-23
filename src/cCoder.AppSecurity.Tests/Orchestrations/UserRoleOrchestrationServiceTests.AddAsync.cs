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

public partial class UserRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        UserRole entity = CreateRandomUserRole();
        userRoleProcessingServiceMock.Setup(expression: x => x.AddUserRoleAsync(entity: entity)).ReturnsAsync(value: entity);

        userRoleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseUserRoleAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        UserRole result = await orchestrationService.AddUserRoleAsync(newUserRole: entity);

        // Then
        result.Should().BeSameAs(expected: entity);
        userRoleProcessingServiceMock.Verify(expression: x => x.AddUserRoleAsync(entity: entity), times: Times.Once);
        userRoleEventProcessingServiceMock.Verify(expression: x => x.RaiseUserRoleAddEventAsync(entity: entity), times: Times.Once);
    }

}