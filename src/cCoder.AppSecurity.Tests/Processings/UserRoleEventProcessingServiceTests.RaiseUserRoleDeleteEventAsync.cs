// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseUserRoleDeleteEventAsync()
    {
        // Given
        UserRole entity = CreateRandomUserRole();
        userRoleEventServiceMock
            .Setup(x => x.RaiseUserRoleDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseUserRoleDeleteEventAsync(entity);

        // Then
        userRoleEventServiceMock.Verify(x => x.RaiseUserRoleDeleteEventAsync(entity), Times.Once);
        userRoleEventServiceMock.VerifyNoOtherCalls();
    }

}