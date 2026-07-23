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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Privilege> entities = new[] { CreateRandomPrivilege() }.AsQueryable();
        privilegeProcessingServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true)).Returns(value: entities);

        // When
        IQueryable<Privilege> result = orchestrationService.GetAll(ignoreFilters: true);

        // Then
        result.Should().BeSameAs(expected: entities);
        privilegeProcessingServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        privilegeProcessingServiceMock.VerifyNoOtherCalls();
        privilegeEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}