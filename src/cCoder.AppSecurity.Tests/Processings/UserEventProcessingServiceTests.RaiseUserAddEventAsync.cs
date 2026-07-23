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
            .Setup(x => x.RaiseUserAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseUserAddEventAsync(entity);

        // Then
        userEventServiceMock.Verify(x => x.RaiseUserAddEventAsync(entity), Times.Once);
        userEventServiceMock.VerifyNoOtherCalls();
    }

}