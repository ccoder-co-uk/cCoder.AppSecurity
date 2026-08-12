// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class PrivilegeServiceExceptionTests
{
    private readonly Mock<IPrivilegeBroker> privilegeBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        UserRoleServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        privilegeBrokerMock
            .Setup(expression: broker => broker.GetAllPrivileges(
                ignoreFilters: false))
            .Throws(exception: exception);

        PrivilegeService service = CreateService();

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
    public async Task ShouldMapAddPrivilegeAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        Privilege privilege = new() { Id = "page_read" };

        privilegeBrokerMock
            .Setup(expression: broker => broker.GetAppId(
                entity: It.Is<Privilege>(match: _ => true)))
            .Throws(exception: exception);

        PrivilegeService service = CreateService();

        // When
        Func<Task> action = async () => await service.AddPrivilegeAsync(
            newPrivilege: privilege);

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
        privilegeBrokerMock
            .Setup(expression: broker => broker.GetAllPrivileges(
                ignoreFilters: false))
            .Returns(value: new[]
            {
                new Privilege { Id = "page_read" }
            }.AsQueryable());

        privilegeBrokerMock
            .Setup(expression: broker => broker.GetAppId(
                entity: It.Is<Privilege>(match: _ => true)))
            .Throws(exception: exception);

        PrivilegeService service = CreateService();

        // When
        Func<Task> action = async () => await service.DeleteAsync(
            privilegeId: "page_read");

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private PrivilegeService CreateService() =>
        new(
            privilegeBroker: privilegeBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);
}