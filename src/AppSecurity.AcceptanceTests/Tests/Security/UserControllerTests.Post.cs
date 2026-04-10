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
        string userId = Unique("user");
        User actualUser;

        // When
        actualUser = await CreateUserAsync(new
        {
            id = userId,
            defaultCultureId = string.Empty,
            displayName = "Acceptance User",
            email = $"{userId}@example.com",
            isActive = true,
        });

        // Then
        actualUser.Id.Should().Be(userId);

        await Teardown(seededContext);
    }
}





