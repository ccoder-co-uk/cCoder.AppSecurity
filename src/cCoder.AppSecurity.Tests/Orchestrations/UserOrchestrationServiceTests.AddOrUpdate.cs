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

public partial class UserOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnProcessingResultsWhenAddOrUpdate()
    {
        // Given
        User[] entities = [CreateRandomUser()];
        Result<User>[] expectedResults = [];
        userProcessingServiceMock.Setup(x => x.AddOrUpdate(entities)).ReturnsAsync(expectedResults);

        // When
        IEnumerable<Result<User>> result = await orchestrationService.AddOrUpdate(entities);

        // Then
        result.Should().BeSameAs(expectedResults);
        userProcessingServiceMock.Verify(x => x.AddOrUpdate(entities), Times.Once);
        userProcessingServiceMock.VerifyNoOtherCalls();
        userEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}