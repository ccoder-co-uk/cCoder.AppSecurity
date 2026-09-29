// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseUserDeleteEventAsync()
    {
        // Given
        User entity = CreateRandomUser();

        userEventServiceMock
            .Setup(expression: x => x.RaiseUserDeleteEventAsync(user: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserDeleteEventAsync(user: entity);

        // Then
        userEventServiceMock.Verify(expression: x => x.RaiseUserDeleteEventAsync(user: entity), times: Times.Once);
        userEventServiceMock.VerifyNoOtherCalls();
    }

}