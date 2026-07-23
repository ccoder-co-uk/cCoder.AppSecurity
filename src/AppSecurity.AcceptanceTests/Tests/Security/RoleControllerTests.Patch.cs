// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class RoleControllerTests
{
    [Fact]
    public async Task Patch_UpdatesRole()
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
        string updatedName = Unique("PatchedRole");
        Role actualRole;

        // When
        await PatchRoleAsync(createdRole.Id, new
        {
            name = updatedName,
            description = "Patched role",
        });

        actualRole = await GetRoleAsync(createdRole.Id);

        // Then
        actualRole.Should().NotBeNull();
        actualRole!.Name.Should().Be(updatedName);

        await DeleteRoleAsync(createdRole.Id);
        await Teardown(seededContext);
    }
}