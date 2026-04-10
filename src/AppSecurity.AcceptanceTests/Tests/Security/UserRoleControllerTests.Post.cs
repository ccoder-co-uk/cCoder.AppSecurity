using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class UserRoleControllerTests
{
    [Fact]
    public async Task Post_CreatesUserRole()
    {
        // Given
        SeededUserRoleContext seededContext = await SeedDatabase();
        UserRole actualUserRole;

        // When
        await CreateUserRoleAsync(new
        {
            userId = seededContext.UserId,
            roleId = seededContext.GuestRoleId,
        });

        actualUserRole = await FindUserRoleAsync(seededContext.UserId, seededContext.GuestRoleId);

        // Then
        actualUserRole.Should().NotBeNull();
        actualUserRole!.UserId.Should().Be(seededContext.UserId);

        await Teardown(seededContext);
    }
}





