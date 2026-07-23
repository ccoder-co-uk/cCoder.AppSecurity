// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class PrivilegeEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePrivilegeUpdateEventAsync()
    {
        // Given
        Privilege entity = CreateRandomPrivilege();
        privilegeEventServiceMock
            .Setup(x => x.RaisePrivilegeUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePrivilegeUpdateEventAsync(entity);

        // Then
        privilegeEventServiceMock.Verify(x => x.RaisePrivilegeUpdateEventAsync(entity), Times.Once);
        privilegeEventServiceMock.VerifyNoOtherCalls();
    }

}