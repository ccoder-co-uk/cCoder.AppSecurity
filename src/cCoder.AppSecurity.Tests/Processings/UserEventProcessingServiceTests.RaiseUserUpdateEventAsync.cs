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
    public async Task ShouldPassThroughCallWhenRaiseUserUpdateEventAsync()
    {
        // Given
        User entity = CreateRandomUser();

        userEventServiceMock
            .Setup(expression: x => x.RaiseUserUpdateEventAsync(user: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserUpdateEventAsync(user: entity);

        // Then
        userEventServiceMock.Verify(expression: x => x.RaiseUserUpdateEventAsync(user: entity), times: Times.Once);
        userEventServiceMock.VerifyNoOtherCalls();
    }

}