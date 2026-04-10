using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseUserRoleAddEventAsync()
    {
        // Given
        UserRole entity = CreateRandomUserRole();
        userRoleEventServiceMock
            .Setup(x => x.RaiseUserRoleAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseUserRoleAddEventAsync(entity);

        // Then
        userRoleEventServiceMock.Verify(x => x.RaiseUserRoleAddEventAsync(entity), Times.Once);
        userRoleEventServiceMock.VerifyNoOtherCalls();
    }

}







