// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Xunit;

namespace AppSecurity.HostedServices.AcceptanceTests.Tests;

public sealed partial class HostedServicesHomeTests
{
    [Fact]
    public async Task Get_ReturnsHostedServicesReport()
    {
        // Given

        // When
        string report = await GetRootAsync();

        // Then
        Assert.Contains(expectedSubstring: "cCoder.AppSecurity Hosted Services", actualString: report);
        Assert.Contains(expectedSubstring: "TokenCleanerHostedService", actualString: report);
        Assert.Contains(expectedSubstring: "AnalysePlatformUsageHostedService", actualString: report);
    }
}