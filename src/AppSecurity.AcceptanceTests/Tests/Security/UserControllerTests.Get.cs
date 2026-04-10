using cCoder.Data;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


using Web.AcceptanceTests.Infrastructure;
namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class UserControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetUserCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfUsers()
    {
        // Given

        // When
        IReadOnlyList<User> actualUsers = await GetUsersAsync(1);

        // Then
        actualUsers.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsUserById()
    {
        // Given
        SeededUserContext seededContext = await SeedDatabase();

        // When
        User actualUser = await GetUserAsync(seededContext.UserId);

        // Then
        actualUser.Should().NotBeNull();
        actualUser!.Id.Should().Be(seededContext.UserId);

        await Teardown(seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededUserContext seededContext = await SeedDatabase("app_admin", "user_create");
        string hiddenUserId = Unique("hidden-user");

        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        await core.AddUserAsync(new User
        {
            Id = hiddenUserId,
            DefaultCultureId = string.Empty,
            DisplayName = "Hidden User",
            Email = $"{hiddenUserId}@example.com",
            IsActive = true,
        });

        User actualUser = await GetUserAsync(hiddenUserId);

        actualUser.Should().BeNull();

        await Teardown(seededContext);
    }
}






