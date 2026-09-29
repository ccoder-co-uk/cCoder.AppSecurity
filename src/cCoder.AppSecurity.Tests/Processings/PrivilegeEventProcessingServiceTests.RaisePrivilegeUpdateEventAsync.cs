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
    public async Task ShouldPassThroughCallWhenRaisePrivilegeUpdateEventAsync()
    {
        // Given
        Privilege entity = CreateRandomPrivilege();

        privilegeEventServiceMock
            .Setup(expression: x => x.RaisePrivilegeUpdateEventAsync(privilege: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaisePrivilegeUpdateEventAsync(privilege: entity);

        // Then
        privilegeEventServiceMock.Verify(expression: x => x.RaisePrivilegeUpdateEventAsync(privilege: entity), times: Times.Once);
        privilegeEventServiceMock.VerifyNoOtherCalls();
    }

}