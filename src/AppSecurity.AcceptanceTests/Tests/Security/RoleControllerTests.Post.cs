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
        string name = Unique("Role");
        Role expectedRole;
        Role actualRole;

        // When
        expectedRole = await CreateRoleAsync(new
        {
            appId = seededContext.AppId,
            name,
            description = "Acceptance role",
            privs = "app_admin",
        });

        actualRole = await GetRoleAsync(expectedRole.Id);

        // Then
        actualRole.Should().NotBeNull();
        actualRole!.Name.Should().Be(name);

        await DeleteRoleAsync(expectedRole.Id);
        await Teardown(seededContext);
    }
}