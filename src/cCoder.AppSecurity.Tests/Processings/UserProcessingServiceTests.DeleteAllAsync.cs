using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachUserWhenSsoUserMatchesForDeleteAllAsync()
    {
        // Given
        User user = CreateRandomUser(id: "test-user");
        coreAuthInfoMock.SetupGet(x => x.SSOUserId).Returns(user.Id);
        userServiceMock.Setup(x => x.Get(user.Id)).Returns(user);
        userServiceMock.Setup(x => x.DeleteAsync(user.Id)).Returns(ValueTask.CompletedTask);

        // When
        await userProcessingService.DeleteAllAsync(new[] { user });

        // Then
        userServiceMock.Verify(x => x.Get(user.Id), Times.Once);
        userServiceMock.Verify(x => x.DeleteAsync(user.Id), Times.Once);
        userServiceMock.VerifyNoOtherCalls();
        coreAuthInfoMock.VerifyGet(x => x.SSOUserId, Times.Once);
        coreAuthInfoMock.VerifyNoOtherCalls();
    }

}






