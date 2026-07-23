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
        privilegeProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<Privilege> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        privilegeProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        privilegeProcessingServiceMock.VerifyNoOtherCalls();
        privilegeEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}