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

public sealed partial class PrivilegeOrchestrationServiceExceptionTests
{
    private readonly Mock<IPrivilegeProcessingService> processingServiceMock = new();
    private readonly Mock<IPrivilegeEventProcessingService> eventServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        new()
        {
            {
                new AppSecurityOrchestrationValidationException(
                    innerException: new ArgumentException()),
                typeof(AppSecurityOrchestrationValidationException)
            },
            {
                new AppSecurityOrchestrationDependencyException(
                    innerException: new InvalidOperationException()),
                typeof(AppSecurityOrchestrationDependencyException)
            },
            {
                new SecurityException(),
                typeof(AppSecurityAuthorizationException)
            },
            {
                new InvalidOperationException(),
                typeof(AppSecurityOrchestrationServiceException)
            }
        };

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        processingServiceMock
            .Setup(expression: service => service.Get(id: "page_read"))
            .Throws(exception: exception);

        PrivilegeOrchestrationService service = CreateService();

        // When
        Action action = () => service.Get(privilegeId: "page_read");

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
    public async Task ShouldMapAddPrivilegeAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Privilege privilege = new() { Id = "page_read" };

        processingServiceMock
            .Setup(expression: service => service.AddPrivilegeAsync(
                entity: privilege))
            .Throws(exception: exception);

        PrivilegeOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () => await service
            .AddPrivilegeAsync(newPrivilege: privilege);

        // Then
        Exception thrownException = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrownException
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapDeleteAllPrivilegeAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Privilege[] privileges = [new() { Id = "page_read" }];

        processingServiceMock
            .Setup(expression: service => service.DeleteAllPrivilegeAsync(
                items: privileges))
            .Throws(exception: exception);

        PrivilegeOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () => await service
            .DeleteAllPrivilegeAsync(deletedPrivilege: privileges);

        // Then
        Exception thrownException = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrownException
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private PrivilegeOrchestrationService CreateService() =>
        new(
            processingService: processingServiceMock.Object,
            eventService: eventServiceMock.Object);
}