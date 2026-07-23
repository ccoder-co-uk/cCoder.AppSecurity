// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class BaselineTests
{
    [Fact]
    public async Task Get_ReturnsPackageArray()
    {
        // Given

        // When
        string response = await Client.GetStringAsync(requestUri: "Api/AppSecurity/Baseline");
        using JsonDocument document = JsonDocument.Parse(json: response);

        // Then
        document.RootElement.ValueKind.Should()
            .Be(expected: JsonValueKind.Array);
    }
}