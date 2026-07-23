// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class UserControllerTests
{
    [Fact]
    public async Task Put_RejectsUpdatingAnotherUser()
    {
        // Given
        SeededUserContext seededContext = await SeedDatabase();

        // When
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}('{Uri.EscapeDataString(seededContext.UserId)}')", new
        {
            id = seededContext.UserId,
            defaultCultureId = string.Empty,
            displayName = "Updated User",
            email = $"{seededContext.UserId}@example.com",
            isActive = true,
        });
        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, content);

        await Teardown(seededContext);
    }
}