using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class PrivilegeEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaisePrivilegeAddEventAsync()
    {
        // Given
        Privilege entity = new();
        EventMessage<cCoder.Data.Models.Security.Privilege> actualMessage = null;

        privilegeEventBrokerMock
            .Setup(x => x.RaisePrivilegeAddEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Security.Privilege>>()))
            .Callback<EventMessage<cCoder.Data.Models.Security.Privilege>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaisePrivilegeAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        privilegeEventBrokerMock.Verify(
            x => x.RaisePrivilegeAddEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Security.Privilege>>()),
            Times.Once
        );
        privilegeEventBrokerMock.VerifyNoOtherCalls();
    }

}








