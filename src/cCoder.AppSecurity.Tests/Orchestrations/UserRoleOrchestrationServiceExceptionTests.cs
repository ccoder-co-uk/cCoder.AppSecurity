// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Orchestrations;

public sealed partial class UserRoleOrchestrationServiceExceptionTests
{
    private readonly Mock<IUserRoleProcessingService> processingServiceMock = new();
    private readonly Mock<IUserRoleEventProcessingService> eventServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        PrivilegeOrchestrationServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        processingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        UserRoleOrchestrationService service = CreateService();

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
    public async Task ShouldMapAddUserRoleAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        UserRole userRole = new() { RoleId = Guid.NewGuid(), UserId = "user-one" };

        processingServiceMock
            .Setup(expression: service => service.AddUserRoleAsync(entity: userRole))
            .Throws(exception: exception);

        UserRoleOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () => await service.AddUserRoleAsync(newUserRole: userRole);

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
    public async Task ShouldMapDeleteAllUserRoleAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        UserRole[] userRoles = [new() { RoleId = Guid.NewGuid(), UserId = "user-one" }];

        processingServiceMock
            .Setup(expression: service => service.DeleteAllUserRoleAsync(items: userRoles))
            .Throws(exception: exception);

        UserRoleOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteAllUserRoleAsync(deletedUserRole: userRoles);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private UserRoleOrchestrationService CreateService() =>
        new(
            processingService: processingServiceMock.Object,
            eventService: eventServiceMock.Object);
}