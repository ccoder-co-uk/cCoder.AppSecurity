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
        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}('{Uri.EscapeDataString(stringToEscape: seededContext.UserId)}')")
        {
            Content = JsonContent.Create(inputValue: new
            {
                displayName = "Patched User",
                isActive = false,
            }),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.Unauthorized, because: content);

        await Teardown(seededContext: seededContext);
    }
}