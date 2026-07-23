// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        IQueryable<DataUser> users = new[] { ToExternalUser(item: user) }.AsQueryable();

        userBrokerMock.Setup(expression: x => x.GetAllUsers(ignoreFilters: false)).Returns(value: users);

        // When
        IQueryable<User> result = userService.GetAll();

        // Then
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(expectation: user);
        userBrokerMock.Verify(expression: x => x.GetAllUsers(ignoreFilters: false), times: Times.Once);
        userBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Security.User>()), times: Times.AtMostOnce());
        userBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}