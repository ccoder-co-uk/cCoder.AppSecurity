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
    public async Task Post_CreatesRole()
    {
        // Given
        SeededRoleContext seededContext = await SeedDatabase();
        string name = Unique(prefix: "Role");
        Role expectedRole;
        Role actualRole;

        // When
        expectedRole = await CreateRoleAsync(payload: new
        {
            appId = seededContext.AppId,
            name,
            description = "Acceptance role",
            privs = "app_admin",
        });

        actualRole = await GetRoleAsync(id: expectedRole.Id);

        // Then
        actualRole.Should()
            .NotBeNull();

        actualRole!.Name.Should()
            .Be(expected: name);

        await DeleteRoleAsync(id: expectedRole.Id);
        await Teardown(seededContext: seededContext);
    }
}