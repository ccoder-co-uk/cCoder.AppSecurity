using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class RoleControllerTests
{
    [Fact]
    public async Task Put_UpdatesRole()
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
        string updatedName = Unique("UpdatedRole");
        Role actualRole;

        // When
        await UpdateRoleAsync(createdRole.Id, new
        {
            id = createdRole.Id,
            appId = seededContext.AppId,
            name = updatedName,
            description = "Updated role",
            privs = "app_admin,script_execute",
        });

        actualRole = await GetRoleAsync(createdRole.Id);

        // Then
        actualRole.Should().NotBeNull();
        actualRole!.Name.Should().Be(updatedName);

        await DeleteRoleAsync(createdRole.Id);
        await Teardown(seededContext);
    }
}





