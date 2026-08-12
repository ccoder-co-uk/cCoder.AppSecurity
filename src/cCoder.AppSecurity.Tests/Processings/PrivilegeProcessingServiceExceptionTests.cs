// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class PrivilegeProcessingServiceExceptionTests
{
    private readonly Mock<IPrivilegeService> privilegeServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        RoleProcessingServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFailure(Exception exception, Type expectedType)
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: null,
                privilege: "privilege_read"));

        privilegeServiceMock
            .Setup(expression: service => service.Get(id: "page_read"))
            .Throws(exception: exception);

        PrivilegeProcessingService service = CreateService();

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
    public async Task ShouldMapAddPrivilegeAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        Privilege privilege = new() { Id = "page_read" };

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: null,
                privilege: "privilege_create"))
            .Throws(exception: exception);

        PrivilegeProcessingService service = CreateService();

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
        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: null,
                privilege: "privilege_delete"))
            .Throws(exception: exception);

        PrivilegeProcessingService service = CreateService();

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

    private PrivilegeProcessingService CreateService() =>
        new(
            service: privilegeServiceMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);
}