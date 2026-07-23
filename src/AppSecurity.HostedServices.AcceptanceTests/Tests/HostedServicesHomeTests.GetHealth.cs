// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Xunit;

namespace AppSecurity.HostedServices.AcceptanceTests.Tests;

public sealed partial class HostedServicesHomeTests
{
    [Fact]
    public async Task GetHealth_ReturnsHealthy()
    {
        // Given

        // When
        string health = await GetHealthAsync();

        // Then
        Assert.Equal("Healthy", health);
    }
}