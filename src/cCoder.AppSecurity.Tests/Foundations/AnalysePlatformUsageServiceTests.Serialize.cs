// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class AnalysePlatformUsageServiceTests
{
    [Fact]
    public void ShouldSerializeValue()
    {
        // Given
        object value = new();
        const string expectedJson = "{}";

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(value: value))
            .Returns(value: expectedJson);

        AnalysePlatformUsageService service = CreateService();

        // When
        string actualJson = service.Serialize(value: value);

        // Then
        actualJson.Should()
            .Be(expected: expectedJson);

        jsonBrokerMock.Verify(
            expression: broker => broker.Serialize(value: value),
            times: Times.Once);
    }
}