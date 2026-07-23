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

public partial class PrivilegeOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnProcessingResultsWhenAddOrUpdate()
    {
        // Given
        Privilege[] entities = [CreateRandomPrivilege()];
        Result<Privilege>[] expectedResults = [];
        privilegeProcessingServiceMock.Setup(x => x.AddOrUpdatePrivilege(entities)).ReturnsAsync(expectedResults);

        // When
        IEnumerable<Result<Privilege>> result = await orchestrationService.AddOrUpdatePrivilege(entities);

        // Then
        result.Should().BeSameAs(expectedResults);
        privilegeProcessingServiceMock.Verify(x => x.AddOrUpdatePrivilege(entities), Times.Once);
        privilegeProcessingServiceMock.VerifyNoOtherCalls();
        privilegeEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}