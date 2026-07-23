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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<User> entities = new[] { CreateRandomUser() }.AsQueryable();

        userProcessingServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<User> result = orchestrationService.GetAll(ignoreFilters: true);

        // Then
        result.Should()
            .BeSameAs(expected: entities);

        userProcessingServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        userProcessingServiceMock.VerifyNoOtherCalls();
        userEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}