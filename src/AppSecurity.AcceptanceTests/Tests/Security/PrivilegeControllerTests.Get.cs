// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class PrivilegeControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsForbidden()
    {
        // Given

        // When
        int actualStatusCode = await GetPrivilegeStatusCodeAsync(relativeUrl: $"{BaseUrl}/$count");

        // Then
        actualStatusCode.Should()
            .Be(expected: 403);
    }

    [Fact]
    public async Task Get_ReturnsPrivilegeById()
    {
        // Given
        string id = Unique(prefix: "privilege");

        await CreatePrivilegeAsync(payload: new
        {
            id,
            type = "Acceptance",
            operation = "Execute",
            description = "Acceptance privilege",
            portalAdminsOnly = false,
        });

        // When
        int actualStatusCode = await GetPrivilegeStatusCodeAsync(relativeUrl: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: id)}')");

        // Then
        actualStatusCode.Should()
            .Be(expected: 403);

        await DeletePrivilegeAsync(id: id);
    }
}