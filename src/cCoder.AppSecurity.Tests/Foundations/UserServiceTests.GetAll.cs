using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        User user = CreateRandomUser();
        IQueryable<DataUser> users = new[] { ToExternalUser(user) }.AsQueryable();

        userBrokerMock.Setup(x => x.GetAllUsers(false)).Returns(users);

        // When
        IQueryable<User> result = userService.GetAll();

        // Then
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(user);
        userBrokerMock.Verify(x => x.GetAllUsers(false), Times.Once);
        userBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>()), Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}








