// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class RoleEventProcessingServiceExceptionTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        RoleProcessingServiceExceptionTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseRoleAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };
        Mock<IRoleEventService> eventServiceMock = new();

        eventServiceMock
            .Setup(expression: service => service.RaiseRoleAddEventAsync(
                entity: role))
            .Throws(exception: exception);

        RoleEventProcessingService service = new(
            eventService: eventServiceMock.Object);

        // When
        Func<Task> action = async () => await service.RaiseRoleAddEventAsync(
            entity: role);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}