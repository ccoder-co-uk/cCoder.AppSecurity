// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class UserControllerTests
{
    [Fact]
    public async Task Delete_RejectsDeletingAnotherUser()
    {
        // Given
        SeededUserContext seededContext = await SeedDatabase();

        // When
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}('{Uri.EscapeDataString(seededContext.UserId)}')");
        string content = await response.Content.ReadAsStringAsync();

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, content);

        await Teardown(seededContext);
    }
}