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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Role> entities = new[] { CreateRandomRole() }.AsQueryable();

        roleProcessingServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<Role> result = orchestrationService.GetAll(ignoreFilters: true);

        // Then
        result.Should()
            .BeSameAs(expected: entities);

        roleProcessingServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}