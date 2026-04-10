using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class UserRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnProcessingResultWhenSaveAsync()
    {
        UserRole entity = CreateRandomUserRole();
        userRoleProcessingServiceMock.Setup(x => x.SaveAsync(entity)).ReturnsAsync(entity);

        UserRole result = await orchestrationService.SaveAsync(entity);

        result.Should().BeSameAs(entity);
        userRoleProcessingServiceMock.Verify(x => x.SaveAsync(entity), Times.Once);
        userRoleProcessingServiceMock.VerifyNoOtherCalls();
        userRoleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







