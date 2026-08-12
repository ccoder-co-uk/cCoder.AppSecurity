// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class AuthorizationServiceTests
{
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        TokenCleanerServiceTests.ExceptionMappings;

    [Fact]
    public void ShouldReturnCurrentUserWhenGetCurrentUser()
    {
        // Given
        User user = new() { Id = "user-one" };

        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: user);

        AuthorizationService service = CreateService();

        // When
        User result = service.GetCurrentUser();

        // Then
        result
            .Should()
            .BeSameAs(expected: user);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetCurrentUserFailure(Exception exception, Type expectedType)
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Throws(exception: exception);

        AuthorizationService service = CreateService();

        // When
        Action action = () => service.GetCurrentUser();

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private AuthorizationService CreateService() =>
        new(authorizationBroker: authorizationBrokerMock.Object);
}