// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Tokens;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class TokenCleanerServiceTests
{
    private readonly Mock<ITokenBroker> tokenBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        new()
        {
            {
                new AppSecurityValidationException(
                    innerException: new ArgumentException()),
                typeof(AppSecurityValidationException)
            },
            {
                new AppSecurityDependencyException(
                    innerException: new InvalidOperationException()),
                typeof(AppSecurityDependencyException)
            },
            { new InvalidOperationException(), typeof(AppSecurityServiceException) }
        };

    [Fact]
    public async Task ShouldDeleteExpiredTokensWhenRunAsync()
    {
        // Given
        CancellationToken cancellationToken = new();

        tokenBrokerMock
            .Setup(expression: broker => broker.DeleteExpiredTokensAsync(
                cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        TokenCleanerService service = CreateService();

        // When
        await service.RunAsync(cancellationToken: cancellationToken);

        // Then
        tokenBrokerMock.Verify(
            expression: broker => broker.DeleteExpiredTokensAsync(
                cancellationToken: cancellationToken),
            times: Times.Once);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRunAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        CancellationToken cancellationToken = new();

        tokenBrokerMock
            .Setup(expression: broker => broker.DeleteExpiredTokensAsync(
                cancellationToken: cancellationToken))
            .Throws(exception: exception);

        TokenCleanerService service = CreateService();

        // When
        Func<Task> action = async () => await service.RunAsync(
            cancellationToken: cancellationToken);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private TokenCleanerService CreateService() =>
        new(tokenBroker: tokenBrokerMock.Object);
}