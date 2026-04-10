using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        roleServiceMock.Setup(x => x.DeleteAsync(roleId)).Returns(ValueTask.CompletedTask);

        // When
        await roleProcessingService.DeleteAsync(roleId);

        // Then
        roleServiceMock.Verify(x => x.DeleteAsync(roleId), Times.Once);
    }

}





