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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        string id = "privilege";
        Privilege entity = CreateRandomPrivilege();
        privilegeProcessingServiceMock.Setup(expression: x => x.Get(id: id)).Returns(value: entity);

        // When
        Privilege result = orchestrationService.Get(privilegeId: id);

        // Then
        result.Should().BeSameAs(expected: entity);
        privilegeProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        privilegeProcessingServiceMock.VerifyNoOtherCalls();
        privilegeEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}