// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class UserRoleProcessingServiceExceptionTests
{
    private readonly Mock<IUserRoleFoundationService> foundationServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        RoleProcessingServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        foundationServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        UserRoleProcessingService service = CreateService();

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
    public async Task ShouldMapSaveUserRoleAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        UserRole userRole = new() { RoleId = Guid.NewGuid(), UserId = "user-one" };

        foundationServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Throws(exception: exception);

        UserRoleProcessingService service = CreateService();

        // When
        Func<Task> action = async () => await service.SaveUserRoleAsync(entity: userRole);

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
    public async Task ShouldMapDeleteUserRoleAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        UserRole userRole = new() { RoleId = Guid.NewGuid(), UserId = "user-one" };

        foundationServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Throws(exception: exception);

        UserRoleProcessingService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteUserRoleAsync(deletedUserRole: userRole);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private UserRoleProcessingService CreateService() =>
        new(service: foundationServiceMock.Object);
}