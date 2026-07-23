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
        User user = CreateRandomUser(id: "user-1");

        userBrokerMock.Setup(expression: x => x.GetAllUsers(ignoreFilters: false))
            .Returns(value: new[] { ToExternalUser(item: user) }.AsQueryable());

        // When
        User result = userService.Get(userId: "user-1");

        // Then
        result.Should()
            .BeEquivalentTo(expectation: user);

        userBrokerMock.Verify(expression: x => x.GetAllUsers(ignoreFilters: false), times: Times.Once);
        userBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}