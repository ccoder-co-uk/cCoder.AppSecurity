// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Storages;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataUserRole = cCoder.Data.Models.Security.UserRole;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class UserRoleServiceExceptionTests
{
    private readonly Mock<IUserRoleBroker> userRoleBrokerMock = new();
    private readonly Mock<IRoleBroker> roleBrokerMock = new();
    private readonly Mock<IUserBroker> userBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        new()
        {
            { new ArgumentException(), typeof(AppSecurityValidationException) },
            {
                new AppSecurityDependencyException(
                    innerException: new InvalidOperationException()),
                typeof(AppSecurityDependencyException)
            },
            { new InvalidOperationException(), typeof(AppSecurityServiceException) }
        };

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        userRoleBrokerMock
            .Setup(expression: broker => broker.GetAllUserRoles(
                ignoreFilters: false))
            .Throws(exception: exception);

        UserRoleService service = CreateService();

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
        UserRole userRole = new()
        {
            RoleId = Guid.NewGuid(),
            UserId = "user-id"
        };

        userRoleBrokerMock
            .Setup(expression: broker => broker.AddUserRoleAsync(
                entity: It.Is<DataUserRole>(match: _ => true)))
            .Throws(exception: exception);

        UserRoleService service = CreateService();

        // When
        Func<Task> action = async () => await service.AddUserRoleAsync(
            newUserRole: userRole,
            authorize: false);

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
        UserRole userRole = new()
        {
            RoleId = Guid.NewGuid(),
            UserId = "user-id"
        };

        userRoleBrokerMock
            .Setup(expression: broker => broker.GetAppId(
                entity: It.Is<DataUserRole>(match: _ => true)))
            .Returns(value: null);

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: null,
                privilege: "UserRole_delete"));

        userRoleBrokerMock
            .Setup(expression: broker => broker.DeleteUserRoleAsync(
                entity: It.Is<DataUserRole>(match: _ => true)))
            .Throws(exception: exception);

        UserRoleService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteUserRoleAsync(
            deletedUserRole: userRole);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private UserRoleService CreateService() =>
        new(
            userRoleBroker: userRoleBrokerMock.Object,
            roleBroker: roleBrokerMock.Object,
            userBroker: userBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);
}