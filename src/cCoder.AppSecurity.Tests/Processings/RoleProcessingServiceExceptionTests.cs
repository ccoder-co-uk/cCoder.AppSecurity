// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class RoleProcessingServiceExceptionTests
{
    private readonly Mock<IRoleService> roleServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        new()
        {
            {
                new AppSecurityProcessingValidationException(
                    innerException: new ArgumentException()),
                typeof(AppSecurityProcessingValidationException)
            },
            {
                new AppSecurityProcessingDependencyException(
                    innerException: new InvalidOperationException()),
                typeof(AppSecurityProcessingDependencyException)
            },
            {
                new InvalidOperationException(),
                typeof(AppSecurityProcessingServiceException)
            }
        };

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        Guid roleId = Guid.NewGuid();

        roleServiceMock
            .Setup(expression: service => service.Get(id: roleId))
            .Throws(exception: exception);

        RoleProcessingService service = CreateService();

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

        roleServiceMock
            .Setup(expression: service => service.AddRoleAsync(role: role))
            .Throws(exception: exception);

        RoleProcessingService service = CreateService();

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
    public async Task ShouldMapDeleteAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        Guid roleId = Guid.NewGuid();

        roleServiceMock
            .Setup(expression: service => service.DeleteAsync(id: roleId))
            .Throws(exception: exception);

        RoleProcessingService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteAsync(roleId: roleId);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private RoleProcessingService CreateService() =>
        new(service: roleServiceMock.Object);
}