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
            .Setup(expression: x => x.RaiseUserRoleDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserRoleDeleteEventAsync(entity: entity);

        // Then
        userRoleEventServiceMock.Verify(expression: x => x.RaiseUserRoleDeleteEventAsync(entity: entity), times: Times.Once);
        userRoleEventServiceMock.VerifyNoOtherCalls();
    }

}