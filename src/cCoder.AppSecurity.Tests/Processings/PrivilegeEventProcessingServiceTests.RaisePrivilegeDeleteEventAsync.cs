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
    public async Task ShouldPassThroughCallWhenRaisePrivilegeDeleteEventAsync()
    {
        // Given
        Privilege entity = CreateRandomPrivilege();
        privilegeEventServiceMock
            .Setup(x => x.RaisePrivilegeDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePrivilegeDeleteEventAsync(entity);

        // Then
        privilegeEventServiceMock.Verify(x => x.RaisePrivilegeDeleteEventAsync(entity), Times.Once);
        privilegeEventServiceMock.VerifyNoOtherCalls();
    }

}