// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Storages;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class AccountRoleAssignmentServiceTests
{
    private readonly Mock<IRoleBroker> roleBrokerMock = new();
    private readonly Mock<IUserRoleBroker> userRoleBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        UserRoleServiceExceptionTests.ExceptionMappings;

    [Fact]
    public void ShouldFindUsersRoleAndAssignmentWhenGetAccountRoleAssignment()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        AccountRoleAssignment assignment = new() { AppId = 7, UserId = "user-one" };

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: true))
            .Returns(value: new[]
            {
                new cCoder.Data.Models.Security.Role
                {
                    Id = roleId,
                    AppId = 7,
                    Name = "Users"
                }
            }.AsQueryable());

        userRoleBrokerMock
            .Setup(expression: broker => broker.GetAllUserRoles(ignoreFilters: true))
            .Returns(value: new[]
            {
                new cCoder.Data.Models.Security.UserRole
                {
                    RoleId = roleId,
                    UserId = assignment.UserId
                }
            }.AsQueryable());

        AccountRoleAssignmentService service = CreateService();

        // When
        AccountRoleAssignment result = service.GetAccountRoleAssignment(
            accountRoleAssignment: assignment);

        // Then
        result.RoleId
            .Should()
            .Be(expected: roleId);

        result.IsAssigned
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task ShouldAddUserRoleWhenAddAccountRoleAssignmentAsync()
    {
        // Given
        AccountRoleAssignment assignment = new()
        {
            AppId = 7,
            UserId = "user-one",
            RoleId = Guid.NewGuid()
        };

        userRoleBrokerMock
            .Setup(expression: broker => broker.AddUserRoleAsync(
                entity: It.Is<cCoder.Data.Models.Security.UserRole>(match: userRole =>
                    userRole.RoleId == assignment.RoleId
                    && userRole.UserId == assignment.UserId)))
            .ReturnsAsync(value: new cCoder.Data.Models.Security.UserRole());

        AccountRoleAssignmentService service = CreateService();

        // When
        AccountRoleAssignment result = await service
            .AddAccountRoleAssignmentAsync(newAccountRoleAssignment: assignment);

        // Then
        result
            .Should()
            .BeSameAs(expected: assignment);
    }

    [Fact]
    public async Task ShouldMapValidationAndUnexpectedFailuresForAccountRoleAssignmentMethods()
    {
        // Given
        AccountRoleAssignmentService service = CreateService();

        // When
        Action getAction = () => service.GetAccountRoleAssignment(
            accountRoleAssignment: null);

        Func<Task> addAction = async () => await service
            .AddAccountRoleAssignmentAsync(newAccountRoleAssignment: null);

        // Then
        getAction
            .Should()
            .Throw<AppSecurityValidationException>();

        await addAction
            .Should()
            .ThrowAsync<AppSecurityValidationException>();
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAccountRoleAssignmentFailure(Exception exception, Type expectedType)
    {
        // Given
        AccountRoleAssignment assignment = new() { AppId = 7, UserId = "user-one" };

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: true))
            .Throws(exception: exception);

        AccountRoleAssignmentService service = CreateService();

        // When
        Action action = () => service.GetAccountRoleAssignment(
            accountRoleAssignment: assignment);

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
    public async Task ShouldMapAddAccountRoleAssignmentAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        AccountRoleAssignment assignment = new()
        {
            AppId = 7,
            UserId = "user-one",
            RoleId = Guid.NewGuid()
        };

        userRoleBrokerMock
            .Setup(expression: broker => broker.AddUserRoleAsync(
                entity: It.Is<cCoder.Data.Models.Security.UserRole>(match: _ => true)))
            .Throws(exception: exception);

        AccountRoleAssignmentService service = CreateService();

        // When
        Func<Task> action = async () => await service
            .AddAccountRoleAssignmentAsync(newAccountRoleAssignment: assignment);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private AccountRoleAssignmentService CreateService() =>
        new(
            roleBroker: roleBrokerMock.Object,
            userRoleBroker: userRoleBrokerMock.Object);
}