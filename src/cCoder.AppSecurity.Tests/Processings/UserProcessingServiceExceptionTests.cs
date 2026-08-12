// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class UserProcessingServiceExceptionTests
{
    private readonly Mock<IUserService> userServiceMock = new();
    private readonly Mock<ICoreAuthInfo> authInfoMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        RoleProcessingServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        userServiceMock
            .Setup(expression: service => service.Get(id: "user-one"))
            .Throws(exception: exception);

        UserProcessingService service = CreateService();

        // When
        Action action = () => service.Get(userId: "user-one");

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

        userServiceMock
            .Setup(expression: service => service.AddUserAsync(user: user))
            .Throws(exception: exception);

        UserProcessingService service = CreateService();

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
        authInfoMock
            .SetupGet(expression: authInfo => authInfo.SSOUserId)
            .Returns(value: "user-one");

        userServiceMock
            .Setup(expression: service => service.Get(id: "user-one"))
            .Returns(value: new User { Id = "user-one" });

        userServiceMock
            .Setup(expression: service => service.DeleteAsync(id: "user-one"))
            .Throws(exception: exception);

        UserProcessingService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteAsync(userId: "user-one");

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private UserProcessingService CreateService() =>
        new(
            service: userServiceMock.Object,
            authInfo: authInfoMock.Object);
}