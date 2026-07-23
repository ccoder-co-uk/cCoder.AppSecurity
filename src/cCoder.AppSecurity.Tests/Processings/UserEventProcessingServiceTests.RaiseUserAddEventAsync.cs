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
    public async Task ShouldPassThroughCallWhenRaiseUserAddEventAsync()
    {
        // Given
        User entity = CreateRandomUser();

        userEventServiceMock
            .Setup(expression: x => x.RaiseUserAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserAddEventAsync(entity: entity);

        // Then
        userEventServiceMock.Verify(expression: x => x.RaiseUserAddEventAsync(entity: entity), times: Times.Once);
        userEventServiceMock.VerifyNoOtherCalls();
    }

}