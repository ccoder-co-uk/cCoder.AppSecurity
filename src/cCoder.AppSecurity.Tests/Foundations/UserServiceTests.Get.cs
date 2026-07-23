// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class UserServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        User user = CreateRandomUser("user-1");

        userBrokerMock.Setup(x => x.GetAllUsers(false)).Returns(new[] { ToExternalUser(user) }.AsQueryable());

        // When
        User result = userService.Get("user-1");

        // Then
        result.Should().BeEquivalentTo(user);
        userBrokerMock.Verify(x => x.GetAllUsers(false), Times.Once);
        userBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Security.User>()), Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}