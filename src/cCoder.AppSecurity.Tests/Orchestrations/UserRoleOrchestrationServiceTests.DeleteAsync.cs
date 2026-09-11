// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class UserRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        UserRole userRole = CreateRandomUserRole();

        userRoleProcessingServiceMock.Setup(expression: x => x.DeleteUserRoleAsync(userRole: userRole))
            .Returns(value: ValueTask.CompletedTask);

        userRoleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseUserRoleDeleteEventAsync(userRole: userRole))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteUserRoleAsync(deletedUserRole: userRole);

        // Then
        userRoleProcessingServiceMock.Verify(expression: x => x.DeleteUserRoleAsync(userRole: userRole), times: Times.Once);
        userRoleEventProcessingServiceMock.Verify(expression: x => x.RaiseUserRoleDeleteEventAsync(userRole: userRole), times: Times.Once);
    }

}