// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        roleProcessingServiceMock.Setup(x => x.AddOrUpdateRole(entities)).ReturnsAsync(expectedResults);

        // When
        IEnumerable<Result<Role>> result = await orchestrationService.AddOrUpdateRole(entities);

        // Then
        result.Should().BeSameAs(expectedResults);
        roleProcessingServiceMock.Verify(x => x.AddOrUpdateRole(entities), Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}