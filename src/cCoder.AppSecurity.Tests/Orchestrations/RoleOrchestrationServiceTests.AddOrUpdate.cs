using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class RoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnProcessingResultsWhenAddOrUpdate()
    {
        // Given
        Role[] entities = [CreateRandomRole()];
        Result<Role>[] expectedResults = [];
        roleProcessingServiceMock.Setup(x => x.AddOrUpdate(entities)).ReturnsAsync(expectedResults);

        // When
        IEnumerable<Result<Role>> result = await orchestrationService.AddOrUpdate(entities);

        // Then
        result.Should().BeSameAs(expectedResults);
        roleProcessingServiceMock.Verify(x => x.AddOrUpdate(entities), Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







