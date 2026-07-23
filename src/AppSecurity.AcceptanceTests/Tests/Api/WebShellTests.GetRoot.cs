// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task GetRoot_RedirectsToTools()
    {
        // Given

        // When
        using HttpResponseMessage response = await GetRootAsync();

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.OriginalString.Should().Be("/tools/index.html");
    }
}