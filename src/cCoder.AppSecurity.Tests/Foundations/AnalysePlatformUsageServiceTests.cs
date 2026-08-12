// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Security;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Security.Data.EF;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class AnalysePlatformUsageServiceTests
{
    private readonly Mock<ISecurityDbContextBroker> securityDbContextBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        TokenCleanerServiceTests.ExceptionMappings;

    [Fact]
    public void ShouldReturnContextWhenCreateSecurityDbContext()
    {
        // Given
        securityDbContextBrokerMock
            .Setup(expression: broker => broker.CreateSecurityDbContext())
            .Returns(value: null);

        AnalysePlatformUsageService service = CreateService();

        // When
        SecurityDbContext result = service.CreateSecurityDbContext();

        // Then
        result
            .Should()
            .BeNull();
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapCreateSecurityDbContextFailure(Exception exception, Type expectedType)
    {
        // Given
        securityDbContextBrokerMock
            .Setup(expression: broker => broker.CreateSecurityDbContext())
            .Throws(exception: exception);

        AnalysePlatformUsageService service = CreateService();

        // When
        Action action = () => service.CreateSecurityDbContext();

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private AnalysePlatformUsageService CreateService() =>
        new(securityDbContextBroker: securityDbContextBrokerMock.Object);
}