using System.Net;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class AppControllerTests
{
    [Fact]
    public async Task Get_ReturnsSeededApps()
    {
        // Given

        // When
        IReadOnlyList<App> actualApps = await GetAppsAsync();

        // Then
        actualApps.Should().ContainSingle();
        actualApps[0].Name.Should().Be("Acceptance");
        actualApps[0].Domain.Should().Be("localhost");
    }

    [Fact]
    public async Task GetCount_ReturnsSeededAppCount()
    {
        // Given

        // When
        int actualCount = await GetAppCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(1);
    }
}
