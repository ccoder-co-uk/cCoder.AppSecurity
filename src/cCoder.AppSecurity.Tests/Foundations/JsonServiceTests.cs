// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class JsonServiceTests
{
    private readonly Mock<IJsonBroker> jsonBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        TokenCleanerServiceTests.ExceptionMappings;

    [Fact]
    public void ShouldReturnParsedValueWhenParseJson()
    {
        // Given
        jsonBrokerMock
            .Setup(expression: broker => broker.ParseJson<string>(json: "\"value\""))
            .Returns(value: "value");

        JsonService service = CreateService();

        // When
        string result = service.ParseJson<string>(json: "\"value\"");

        // Then
        result
            .Should()
            .Be(expected: "value");
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapParseJsonFailure(Exception exception, Type expectedType)
    {
        // Given
        jsonBrokerMock
            .Setup(expression: broker => broker.ParseJson<string>(json: "\"value\""))
            .Throws(exception: exception);

        JsonService service = CreateService();

        // When
        Action action = () => service.ParseJson<string>(json: "\"value\"");

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Fact]
    public void ShouldReturnJsonWhenSerialize()
    {
        // Given
        object value = new();

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(value: value))
            .Returns(value: "{}");

        JsonService service = CreateService();

        // When
        string result = service.Serialize(value: value);

        // Then
        result
            .Should()
            .Be(expected: "{}");
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapSerializeFailure(Exception exception, Type expectedType)
    {
        // Given
        object value = new();

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(value: value))
            .Throws(exception: exception);

        JsonService service = CreateService();

        // When
        Action action = () => service.Serialize(value: value);

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private JsonService CreateService() =>
        new(jsonBroker: jsonBrokerMock.Object);
}