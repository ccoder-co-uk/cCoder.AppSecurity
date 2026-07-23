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
            .Setup(expression: x => x.RaisePrivilegeDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePrivilegeDeleteEventAsync(entity: entity);

        // Then
        privilegeEventServiceMock.Verify(expression: x => x.RaisePrivilegeDeleteEventAsync(entity: entity), times: Times.Once);
        privilegeEventServiceMock.VerifyNoOtherCalls();
    }

}