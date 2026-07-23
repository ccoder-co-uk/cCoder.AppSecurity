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
        roleProcessingServiceMock.Setup(expression: x => x.Get(id: id)).Returns(value: entity);

        // When
        Role result = orchestrationService.Get(roleId: id);

        // Then
        result.Should().BeSameAs(expected: entity);
        roleProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}