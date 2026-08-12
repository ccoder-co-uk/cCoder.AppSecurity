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

public sealed partial class PrivilegeEventServiceExceptionTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        UserEventServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaisePrivilegeAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Mock<IPrivilegeEventBroker> eventBrokerMock = new();
        Mock<IAuthInfoBroker> authInfoBrokerMock = new();

        authInfoBrokerMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: "user-one");

        eventBrokerMock
            .Setup(expression: broker => broker.RaisePrivilegeAddEventAsync(
                message: It.Is<cCoder.Eventing.Models.EventMessage<Privilege>>(
                    match: _ => true)))
            .Throws(exception: exception);

        PrivilegeEventService service = new(
            privilegeEventBroker: eventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object);

        // When
        Func<Task> action = async () => await service.RaisePrivilegeAddEventAsync(
            entity: new Privilege());

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}