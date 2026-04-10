using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
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
            .Setup(x => x.RaiseUserUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseUserUpdateEventAsync(entity);

        // Then
        userEventServiceMock.Verify(x => x.RaiseUserUpdateEventAsync(entity), Times.Once);
        userEventServiceMock.VerifyNoOtherCalls();
    }

}







