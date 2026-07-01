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
        SeededRoleContext seededContext = await SeedDatabase(
            "app_admin",
            "role_create",
            "role_update",
            "role_delete",
            "role_read",
            "script_execute");
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

    [Fact]
    public async Task Put_RejectsGrantingPrivilegeActorDoesNotHave()
    {
        // Given
        SeededRoleContext seededContext = await SeedDatabase(
            "app_admin",
            "role_create",
            "role_update",
            "role_delete",
            "role_read");
        Role createdRole = await CreateRoleAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("Role"),
            description = "Acceptance role",
            privs = "app_admin",
        });

        // When
        (int statusCode, string content) = await PutRoleAsync(createdRole.Id, new
        {
            id = createdRole.Id,
            appId = seededContext.AppId,
            name = createdRole.Name,
            description = "Escalated role",
            privs = "app_admin,script_execute",
        });

        Role actualRole = await GetRoleAsync(createdRole.Id);

        // Then
        statusCode.Should().Be(401, content);
        content.Should().Contain("Access Denied!");
        actualRole.Should().NotBeNull();
        actualRole!.Privs.Should().Be("app_admin");

        await DeleteRoleAsync(createdRole.Id);
        await Teardown(seededContext);
    }
}





