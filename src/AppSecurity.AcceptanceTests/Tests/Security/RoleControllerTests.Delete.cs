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
    public async Task Delete_RemovesRole()
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

        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteRoleAsync(id: createdRole.Id);
        actualReadStatusCode = await GetRoleStatusCodeAsync(id: createdRole.Id);

        // Then
        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}