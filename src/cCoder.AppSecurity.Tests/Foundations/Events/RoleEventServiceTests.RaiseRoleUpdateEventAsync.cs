// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class RoleEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseRoleUpdateEventAsync()
    {
        // Given
        Role entity = new();
        EventMessage<cCoder.Data.Models.Security.Role> actualMessage = null;

        roleEventBrokerMock
            .Setup(expression: x => x.RaiseRoleUpdateEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Security.Role>>()))
            .Callback<EventMessage<cCoder.Data.Models.Security.Role>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseRoleUpdateEventAsync(entity: entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(
expectation:             entity,
config:             options => options.Excluding(expression: candidate => candidate.Privileges)
        );
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(expected: CurrentUserId);
        roleEventBrokerMock.Verify(
expression:             x => x.RaiseRoleUpdateEventAsync(message: It.IsAny<EventMessage<cCoder.Data.Models.Security.Role>>()),
times:             Times.Once
        );
        roleEventBrokerMock.VerifyNoOtherCalls();
    }

}