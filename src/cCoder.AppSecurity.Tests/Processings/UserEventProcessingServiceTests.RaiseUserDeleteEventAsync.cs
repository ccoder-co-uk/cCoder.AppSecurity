// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
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
            .Setup(expression: x => x.RaiseUserDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserDeleteEventAsync(entity: entity);

        // Then
        userEventServiceMock.Verify(expression: x => x.RaiseUserDeleteEventAsync(entity: entity), times: Times.Once);
        userEventServiceMock.VerifyNoOtherCalls();
    }

}