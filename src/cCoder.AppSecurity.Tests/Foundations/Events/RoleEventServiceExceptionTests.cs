// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations.Events;

public sealed partial class RoleEventServiceExceptionTests
{
    [Theory]
    [InlineData(typeof(ArgumentException), typeof(AppSecurityValidationException))]
    [InlineData(typeof(AppSecurityDependencyException), typeof(AppSecurityDependencyException))]
    [InlineData(typeof(InvalidOperationException), typeof(AppSecurityServiceException))]
    public async Task ShouldMapRaiseRoleAddEventAsyncFailure(
        Type exceptionType,
        Type expectedType)
    {
        // Given
        Exception exception = exceptionType == typeof(AppSecurityDependencyException)
            ? new AppSecurityDependencyException(innerException: new InvalidOperationException())
            : (Exception)Activator.CreateInstance(type: exceptionType);

        Mock<IRoleEventBroker> eventBrokerMock = new();
        Mock<IAuthInfoBroker> authInfoBrokerMock = new();

        authInfoBrokerMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: "user-one");

        eventBrokerMock
            .Setup(expression: broker => broker.RaiseRoleAddEventAsync(
                message: It.IsAny<cCoder.Eventing.Models.EventMessage<Role>>()))
            .Throws(exception: exception);

        var service = new RoleEventService(
            roleEventBroker: eventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object);

        // When
        Func<Task> action = async () => await service.RaiseRoleAddEventAsync(
            entity: new Role());

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}