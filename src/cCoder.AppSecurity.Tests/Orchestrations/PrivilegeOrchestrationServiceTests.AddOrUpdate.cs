using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class PrivilegeOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnProcessingResultsWhenAddOrUpdate()
    {
        // Given
        Privilege[] entities = [CreateRandomPrivilege()];
        Result<Privilege>[] expectedResults = [];
        privilegeProcessingServiceMock.Setup(x => x.AddOrUpdate(entities)).ReturnsAsync(expectedResults);

        // When
        IEnumerable<Result<Privilege>> result = await orchestrationService.AddOrUpdate(entities);

        // Then
        result.Should().BeSameAs(expectedResults);
        privilegeProcessingServiceMock.Verify(x => x.AddOrUpdate(entities), Times.Once);
        privilegeProcessingServiceMock.VerifyNoOtherCalls();
        privilegeEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







