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
            .Setup(x => x.RaiseRoleUpdateEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Security.Role>>()))
            .Callback<EventMessage<cCoder.Data.Models.Security.Role>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseRoleUpdateEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(
            entity,
            options => options.Excluding(candidate => candidate.Privileges)
        );
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        roleEventBrokerMock.Verify(
            x => x.RaiseRoleUpdateEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Security.Role>>()),
            Times.Once
        );
        roleEventBrokerMock.VerifyNoOtherCalls();
    }

}