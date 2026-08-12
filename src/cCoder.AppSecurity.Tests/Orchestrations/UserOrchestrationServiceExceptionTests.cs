// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Orchestrations;

public sealed partial class UserOrchestrationServiceExceptionTests
{
    private readonly Mock<IUserProcessingService> processingServiceMock = new();
    private readonly Mock<IUserEventProcessingService> eventServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        PrivilegeOrchestrationServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        processingServiceMock
            .Setup(expression: service => service.Get(id: "user-one"))
            .Throws(exception: exception);

        UserOrchestrationService service = CreateService();

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

        processingServiceMock
            .Setup(expression: service => service.AddUserAsync(entity: user))
            .Throws(exception: exception);

        UserOrchestrationService service = CreateService();

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
    public async Task ShouldMapDeleteAllUserAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        User[] users = [new() { Id = "user-one" }];

        processingServiceMock
            .Setup(expression: service => service.DeleteAllUserAsync(items: users))
            .Throws(exception: exception);

        UserOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteAllUserAsync(deletedUser: users);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private UserOrchestrationService CreateService() =>
        new(
            processingService: processingServiceMock.Object,
            eventService: eventServiceMock.Object);
}