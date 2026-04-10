using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseRoleDeleteEventAsync()
    {
        // Given
        Role entity = CreateRandomRole();
        roleEventServiceMock
            .Setup(x => x.RaiseRoleDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseRoleDeleteEventAsync(entity);

        // Then
        roleEventServiceMock.Verify(x => x.RaiseRoleDeleteEventAsync(entity), Times.Once);
        roleEventServiceMock.VerifyNoOtherCalls();
    }

}







