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

        Role createdRole = await CreateRoleAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique(prefix: "Role"),
            description = "Acceptance role",
            privs = "app_admin",
        });

        string updatedName = Unique(prefix: "PatchedRole");
        Role actualRole;

        // When
        await PatchRoleAsync(id: createdRole.Id, payload: new
        {
            name = updatedName,
            description = "Patched role",
        });

        actualRole = await GetRoleAsync(id: createdRole.Id);

        // Then
        actualRole.Should()
            .NotBeNull();

        actualRole!.Name.Should()
            .Be(expected: updatedName);

        await DeleteRoleAsync(id: createdRole.Id);
        await Teardown(seededContext: seededContext);
    }
}