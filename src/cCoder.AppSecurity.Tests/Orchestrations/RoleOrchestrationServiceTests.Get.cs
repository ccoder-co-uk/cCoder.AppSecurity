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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        Guid id = Guid.NewGuid();
        Role entity = CreateRandomRole();
        roleProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        Role result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        roleProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}