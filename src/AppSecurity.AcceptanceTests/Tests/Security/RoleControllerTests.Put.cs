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
    public async Task Put_UpdatesRole()
    {
        // Given
        string[] privileges =
        [
            "app_admin",
            "role_create",
            "role_update",
            "role_delete",
            "role_read",
            "script_execute",
        ];

        SeededRoleContext seededContext = await SeedDatabase(
            privileges: privileges);

        Role createdRole = await CreateRoleAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique(prefix: "Role"),
            description = "Acceptance role",
            privs = "app_admin",
        });

        string updatedName = Unique(prefix: "UpdatedRole");
        Role actualRole;

        // When
        await UpdateRoleAsync(id: createdRole.Id, payload: new
        {
            id = createdRole.Id,
            appId = seededContext.AppId,
            name = updatedName,
            description = "Updated role",
            privs = "app_admin,script_execute",
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

    [Fact]
    public async Task Put_RejectsGrantingPrivilegeActorDoesNotHave()
    {
        // Given
        string[] privileges =
        [
            "app_admin",
            "role_create",
            "role_update",
            "role_delete",
            "role_read",
        ];

        SeededRoleContext seededContext = await SeedDatabase(
            privileges: privileges);

        Role createdRole = await CreateRoleAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique(prefix: "Role"),
            description = "Acceptance role",
            privs = "app_admin",
        });

        // When
        (int statusCode, string content) = await PutRoleAsync(id: createdRole.Id, payload: new
        {
            id = createdRole.Id,
            appId = seededContext.AppId,
            name = createdRole.Name,
            description = "Escalated role",
            privs = "app_admin,script_execute",
        });

        Role actualRole = await GetRoleAsync(id: createdRole.Id);

        // Then
        statusCode.Should()
            .Be(expected: 401, because: content);

        content.Should()
            .Contain(expected: "Access Denied!");

        actualRole.Should()
            .NotBeNull();

        actualRole!.Privs.Should()
            .Be(expected: "app_admin");

        await DeleteRoleAsync(id: createdRole.Id);
        await Teardown(seededContext: seededContext);
    }
}