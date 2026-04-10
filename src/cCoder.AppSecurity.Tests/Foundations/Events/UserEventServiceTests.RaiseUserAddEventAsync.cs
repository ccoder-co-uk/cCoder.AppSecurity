using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using EventLibrary.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class UserEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseUserAddEventAsync()
    {
        // Given
        User entity = new();
        EventMessage<cCoder.Data.Models.Security.User> actualMessage = null;

        userEventBrokerMock
            .Setup(x => x.RaiseUserAddEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Security.User>>()))
            .Callback<EventMessage<cCoder.Data.Models.Security.User>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseUserAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        userEventBrokerMock.Verify(
            x => x.RaiseUserAddEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Security.User>>()),
            Times.Once
        );
        userEventBrokerMock.VerifyNoOtherCalls();
    }

}








