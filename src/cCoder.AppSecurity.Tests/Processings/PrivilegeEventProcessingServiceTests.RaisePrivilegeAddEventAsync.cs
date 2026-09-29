// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class PrivilegeEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaisePrivilegeAddEventAsync()
    {
        // Given
        Privilege entity = CreateRandomPrivilege();

        privilegeEventServiceMock
            .Setup(expression: x => x.RaisePrivilegeAddEventAsync(privilege: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePrivilegeAddEventAsync(privilege: entity);

        // Then
        privilegeEventServiceMock.Verify(expression: x => x.RaisePrivilegeAddEventAsync(privilege: entity), times: Times.Once);
        privilegeEventServiceMock.VerifyNoOtherCalls();
    }

}