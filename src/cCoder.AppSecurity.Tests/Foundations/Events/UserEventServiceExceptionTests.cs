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

public sealed partial class UserEventServiceExceptionTests
{
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
    public async Task ShouldMapRaiseUserAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Mock<IUserEventBroker> eventBrokerMock = new();
        Mock<IAuthInfoBroker> authInfoBrokerMock = new();

        authInfoBrokerMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: "user-one");

        eventBrokerMock
            .Setup(expression: broker => broker.RaiseUserAddEventAsync(
                message: It.Is<cCoder.Eventing.Models.EventMessage<User>>(
                    match: _ => true)))
            .Throws(exception: exception);

        UserEventService service = new(
            userEventBroker: eventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object);

        // When
        Func<Task> action = async () => await service.RaiseUserAddEventAsync(
            entity: new User());

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}