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
            .Setup(x => x.RaiseUserDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseUserDeleteEventAsync(entity);

        // Then
        userEventServiceMock.Verify(x => x.RaiseUserDeleteEventAsync(entity), Times.Once);
        userEventServiceMock.VerifyNoOtherCalls();
    }

}