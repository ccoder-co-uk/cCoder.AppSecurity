using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class PrivilegeEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePrivilegeAddEventAsync()
    {
        // Given
        Privilege entity = CreateRandomPrivilege();
        privilegeEventServiceMock
            .Setup(x => x.RaisePrivilegeAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePrivilegeAddEventAsync(entity);

        // Then
        privilegeEventServiceMock.Verify(x => x.RaisePrivilegeAddEventAsync(entity), Times.Once);
        privilegeEventServiceMock.VerifyNoOtherCalls();
    }

}







