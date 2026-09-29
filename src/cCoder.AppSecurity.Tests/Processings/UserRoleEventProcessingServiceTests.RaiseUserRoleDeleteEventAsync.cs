// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
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
            .Setup(expression: x => x.RaiseUserRoleDeleteEventAsync(userRole: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserRoleDeleteEventAsync(userRole: entity);

        // Then
        userRoleEventServiceMock.Verify(expression: x => x.RaiseUserRoleDeleteEventAsync(userRole: entity), times: Times.Once);
        userRoleEventServiceMock.VerifyNoOtherCalls();
    }

}