// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseRoleAddEventAsync()
    {
        // Given
        Role entity = CreateRandomRole();

        roleEventServiceMock
            .Setup(expression: x => x.RaiseRoleAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseRoleAddEventAsync(entity: entity);

        // Then
        roleEventServiceMock.Verify(expression: x => x.RaiseRoleAddEventAsync(entity: entity), times: Times.Once);
        roleEventServiceMock.VerifyNoOtherCalls();
    }

}