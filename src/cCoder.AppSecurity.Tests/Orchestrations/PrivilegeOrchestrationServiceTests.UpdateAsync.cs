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
        privilegeProcessingServiceMock.Setup(x => x.UpdatePrivilegeAsync(entity)).ReturnsAsync(entity);

        privilegeEventProcessingServiceMock
            .Setup(x => x.RaisePrivilegeUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Privilege result = await orchestrationService.UpdatePrivilegeAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        privilegeProcessingServiceMock.Verify(x => x.UpdatePrivilegeAsync(entity), Times.Once);
        privilegeEventProcessingServiceMock.Verify(x => x.RaisePrivilegeUpdateEventAsync(entity), Times.Once);
    }

}