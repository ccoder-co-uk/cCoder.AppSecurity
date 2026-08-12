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

public sealed partial class RoleOrchestrationServiceExceptionTests
{
    private readonly Mock<IRoleProcessingService> processingServiceMock = new();
    private readonly Mock<IRoleEventProcessingService> eventServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        PrivilegeOrchestrationServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        Guid roleId = Guid.NewGuid();

        processingServiceMock
            .Setup(expression: service => service.Get(id: roleId))
            .Throws(exception: exception);

        RoleOrchestrationService service = CreateService();

        // When
        Action action = () => service.Get(roleId: roleId);

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
    public async Task ShouldMapAddRoleAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };

        processingServiceMock
            .Setup(expression: service => service.AddRoleAsync(entity: role))
            .Throws(exception: exception);

        RoleOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () => await service.AddRoleAsync(newRole: role);

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
    public async Task ShouldMapDeleteAllRoleAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        Role[] roles = [new() { Id = Guid.NewGuid() }];

        processingServiceMock
            .Setup(expression: service => service.DeleteAllRoleAsync(items: roles))
            .Throws(exception: exception);

        RoleOrchestrationService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteAllRoleAsync(deletedRole: roles);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private RoleOrchestrationService CreateService() =>
        new(
            processingService: processingServiceMock.Object,
            eventService: eventServiceMock.Object);
}