// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        actualApps.Should()
            .ContainSingle();

        actualApps[index: 0].Name.Should()
            .Be(expected: "Acceptance");

        actualApps[index: 0].Domain.Should()
            .Be(expected: "localhost");
    }

    [Fact]
    public async Task GetCount_ReturnsSeededAppCount()
    {
        // Given

        // When
        int actualCount = await GetAppCountAsync();

        // Then
        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 1);
    }
}