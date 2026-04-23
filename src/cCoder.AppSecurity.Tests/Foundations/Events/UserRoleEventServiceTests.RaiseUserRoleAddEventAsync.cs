using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class UserRoleEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseUserRoleAddEventAsync()
    {
        // Given
        UserRole entity = new();
        EventMessage<cCoder.Data.Models.Security.UserRole> actualMessage = null;

        userRoleEventBrokerMock
            .Setup(x => x.RaiseUserRoleAddEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Security.UserRole>>()))
            .Callback<EventMessage<cCoder.Data.Models.Security.UserRole>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseUserRoleAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Should().BeEquivalentTo(entity);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        userRoleEventBrokerMock.Verify(
            x => x.RaiseUserRoleAddEventAsync(It.IsAny<EventMessage<cCoder.Data.Models.Security.UserRole>>()),
            Times.Once
        );
        userRoleEventBrokerMock.VerifyNoOtherCalls();
    }

}










