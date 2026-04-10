using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class RoleControllerTests
{
    [Fact]
    public async Task Delete_RemovesRole()
    {
        // Given
        SeededRoleContext seededContext = await SeedDatabase();
        Role createdRole = await CreateRoleAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("Role"),
            description = "Acceptance role",
            privs = "app_admin",
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteRoleAsync(createdRole.Id);
        actualReadStatusCode = await GetRoleStatusCodeAsync(createdRole.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





