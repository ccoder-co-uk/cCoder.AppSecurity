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
    public async Task Patch_RejectsUpdatingAnotherUser()
    {
        // Given
        SeededUserContext seededContext = await SeedDatabase();

        // When
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}('{Uri.EscapeDataString(seededContext.UserId)}')")
        {
            Content = JsonContent.Create(new
            {
                displayName = "Patched User",
                isActive = false,
            }),
        };
        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, content);

        await Teardown(seededContext);
    }
}