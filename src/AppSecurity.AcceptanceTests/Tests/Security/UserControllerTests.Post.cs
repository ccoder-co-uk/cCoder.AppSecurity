// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class UserControllerTests
{
    [Fact]
    public async Task Post_CreatesUser()
    {
        // Given
        SeededUserContext seededContext = await SeedDatabase();
        string userId = Unique(prefix: "user");
        User actualUser;

        // When
        actualUser = await CreateUserAsync(payload: new
        {
            id = userId,
            defaultCultureId = string.Empty,
            displayName = "Acceptance User",
            email = $"{userId}@example.com",
            isActive = true,
        });

        // Then
        actualUser.Id.Should()
            .Be(expected: userId);

        await Teardown(seededContext: seededContext);
    }
}