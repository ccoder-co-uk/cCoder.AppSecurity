// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class JsonProcessingServiceTests
{
    private readonly Mock<IJsonService> jsonServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        RoleProcessingServiceExceptionTests.ExceptionMappings;

    [Fact]
    public void ShouldReturnParsedValueWhenParseJson()
    {
        // Given
        jsonServiceMock
            .Setup(expression: service => service.ParseJson<string>(
                json: "\"value\""))
            .Returns(value: "value");

        JsonProcessingService service = CreateService();

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
        jsonServiceMock
            .Setup(expression: service => service.ParseJson<string>(
                json: "\"value\""))
            .Throws(exception: exception);

        JsonProcessingService service = CreateService();

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

        jsonServiceMock
            .Setup(expression: service => service.Serialize(value: value))
            .Returns(value: "{}");

        JsonProcessingService service = CreateService();

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

        jsonServiceMock
            .Setup(expression: service => service.Serialize(value: value))
            .Throws(exception: exception);

        JsonProcessingService service = CreateService();

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

    private JsonProcessingService CreateService() =>
        new(jsonService: jsonServiceMock.Object);
}