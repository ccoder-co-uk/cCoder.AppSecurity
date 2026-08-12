// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations.Events;

public sealed partial class UserRoleEventServiceExceptionTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        UserEventServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseUserRoleAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Mock<IUserRoleEventBroker> eventBrokerMock = new();
        Mock<IAuthInfoBroker> authInfoBrokerMock = new();

        authInfoBrokerMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: "user-one");

        eventBrokerMock
            .Setup(expression: broker => broker.RaiseUserRoleAddEventAsync(
                message: It.Is<cCoder.Eventing.Models.EventMessage<UserRole>>(
                    match: _ => true)))
            .Throws(exception: exception);

        UserRoleEventService service = new(
            userRoleEventBroker: eventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object);

        // When
        Func<Task> action = async () => await service.RaiseUserRoleAddEventAsync(
            entity: new UserRole());

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}