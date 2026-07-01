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
        Assert.Contains("cCoder.AppSecurity Hosted Services", report);
        Assert.Contains("TokenCleanerHostedService", report);
        Assert.Contains("AnalysePlatformUsageHostedService", report);
    }
}
