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
        roleProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<Role> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        roleProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}