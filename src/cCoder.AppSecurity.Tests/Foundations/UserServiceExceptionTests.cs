// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Storages;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class UserServiceExceptionTests
{
    private readonly Mock<IUserBroker> userBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        UserRoleServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        userBrokerMock
            .Setup(expression: broker => broker.GetAllUsers(ignoreFilters: false))
            .Throws(exception: exception);

        UserService service = CreateService();

        // When
        Action action = () => service.GetAll();

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapAddUserAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        User user = new() { Id = "user-one" };

        userBrokerMock
            .Setup(expression: broker => broker.GetAllUsers(ignoreFilters: true))
            .Returns(value: new[]
            {
                new User { Id = "existing-user" }
            }.AsQueryable());

        userBrokerMock
            .Setup(expression: broker => broker.GetAppId(
                entity: It.Is<User>(match: _ => true)))
            .Throws(exception: exception);

        UserService service = CreateService();

        // When
        Func<Task> action = async () => await service.AddUserAsync(newUser: user);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapDeleteAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        User user = new() { Id = "user-one" };

        userBrokerMock
            .Setup(expression: broker => broker.GetAllUsers(ignoreFilters: false))
            .Returns(value: new[] { user }.AsQueryable());

        userBrokerMock
            .Setup(expression: broker => broker.GetAppId(
                entity: It.Is<User>(match: mapped => mapped.Id == user.Id)))
            .Throws(exception: exception);

        UserService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteAsync(userId: user.Id);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private UserService CreateService() =>
        new(
            userBroker: userBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);
}