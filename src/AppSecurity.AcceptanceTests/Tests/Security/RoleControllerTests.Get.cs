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
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfRoles()
    {
        // Given

        // When
        IReadOnlyList<Role> actualRoles = await GetRolesAsync(1);

        // Then
        actualRoles.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsRoleById()
    {
        // Given
        SeededRoleContext seededContext = await SeedDatabase();
        string name = Unique("Role");
        Role expectedRole = await CreateRoleAsync(new
        {
            appId = seededContext.AppId,
            name,
            description = "Acceptance role",
            privs = "app_admin",
        });
        Role actualRole;

        // When
        actualRole = await GetRoleAsync(expectedRole.Id);

        // Then
        actualRole.Should().NotBeNull();
        actualRole!.Id.Should().Be(expectedRole.Id);
        actualRole.Name.Should().Be(name);

        await DeleteRoleAsync(expectedRole.Id);
        await Teardown(seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededRoleContext seededContext = await SeedDatabase("role_create", "role_update", "role_delete");

        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Role hiddenRole = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = seededContext.AppId,
            Name = Unique("HiddenRole"),
            Description = "Hidden role",
            Privs = "role_update",
        });

        Role actualRole = await GetRoleAsync(hiddenRole.Id);

        actualRole.Should().BeNull();

        await Teardown(seededContext);
    }
}






