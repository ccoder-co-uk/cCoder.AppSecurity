// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


using Web.AcceptanceTests.Infrastructure;
namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class RoleControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetRoleCountAsync();

        // Then
        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfRoles()
    {
        // Given

        // When
        IReadOnlyList<Role> actualRoles = await GetRolesAsync(top: 1);

        // Then
        actualRoles.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsRoleById()
    {
        // Given
        SeededRoleContext seededContext = await SeedDatabase();
        string name = Unique(prefix: "Role");

        Role expectedRole = await CreateRoleAsync(payload: new
        {
            appId = seededContext.AppId,
            name,
            description = "Acceptance role",
            privs = "app_admin",
        });

        Role actualRole;

        // When
        actualRole = await GetRoleAsync(id: expectedRole.Id);

        // Then
        actualRole.Should()
            .NotBeNull();

        actualRole!.Id.Should()
            .Be(expected: expectedRole.Id);

        actualRole.Name.Should()
            .Be(expected: name);

        await DeleteRoleAsync(id: expectedRole.Id);
        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        // Given
        string[] privileges =
        [
            "role_create",
            "role_update",
            "role_delete",
        ];

        SeededRoleContext seededContext = await SeedDatabase(
            privileges: privileges);

        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Role hiddenRole = new()
        {
            Id = Guid.NewGuid(),
            AppId = seededContext.AppId,
            Name = Unique(prefix: "HiddenRole"),
            Description = "Hidden role",
            Privs = "role_update",
        };

        hiddenRole = await core.AddRoleAsync(role: hiddenRole);

        // When
        Role actualRole = await GetRoleAsync(id: hiddenRole.Id);

        // Then
        actualRole.Should()
            .BeNull();

        await Teardown(seededContext: seededContext);
    }
}