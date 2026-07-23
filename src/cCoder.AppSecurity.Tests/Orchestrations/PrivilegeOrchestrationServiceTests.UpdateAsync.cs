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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Privilege entity = CreateRandomPrivilege();
        privilegeProcessingServiceMock.Setup(expression: x => x.UpdatePrivilegeAsync(entity: entity)).ReturnsAsync(value: entity);

        privilegeEventProcessingServiceMock
            .Setup(expression: x => x.RaisePrivilegeUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Privilege result = await orchestrationService.UpdatePrivilegeAsync(updatedPrivilege: entity);

        // Then
        result.Should().BeSameAs(expected: entity);
        privilegeProcessingServiceMock.Verify(expression: x => x.UpdatePrivilegeAsync(entity: entity), times: Times.Once);
        privilegeEventProcessingServiceMock.Verify(expression: x => x.RaisePrivilegeUpdateEventAsync(entity: entity), times: Times.Once);
    }

}