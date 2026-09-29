// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseRoleUpdateEventAsync()
    {
        // Given
        Role entity = CreateRandomRole();

        roleEventServiceMock
            .Setup(expression: x => x.RaiseRoleUpdateEventAsync(role: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseRoleUpdateEventAsync(role: entity);

        // Then
        roleEventServiceMock.Verify(expression: x => x.RaiseRoleUpdateEventAsync(role: entity), times: Times.Once);
        roleEventServiceMock.VerifyNoOtherCalls();
    }

}