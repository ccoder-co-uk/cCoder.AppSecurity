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
        userRoleProcessingServiceMock.Setup(x => x.DeleteUserRoleAsync(userRole)).Returns(ValueTask.CompletedTask);

        userRoleEventProcessingServiceMock
            .Setup(x => x.RaiseUserRoleDeleteEventAsync(userRole))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteUserRoleAsync(userRole);

        // Then
        userRoleProcessingServiceMock.Verify(x => x.DeleteUserRoleAsync(userRole), Times.Once);
        userRoleEventProcessingServiceMock.Verify(x => x.RaiseUserRoleDeleteEventAsync(userRole), Times.Once);
    }

}