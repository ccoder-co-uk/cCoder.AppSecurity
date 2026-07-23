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

public sealed partial class UserControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetUserCountAsync();

        // Then
        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfUsers()
    {
        // Given

        // When
        IReadOnlyList<User> actualUsers = await GetUsersAsync(top: 1);

        // Then
        actualUsers.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsUserById()
    {
        // Given
        SeededUserContext seededContext = await SeedDatabase();

        // When
        User actualUser = await GetUserAsync(id: seededContext.UserId);

        // Then
        actualUser.Should()
            .NotBeNull();

        actualUser!.Id.Should()
            .Be(expected: seededContext.UserId);

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        // Given
        string[] guestPrivileges =
        [
            "app_admin",
            "user_create",
        ];

        SeededUserContext seededContext = await SeedDatabase(
            guestPrivileges: guestPrivileges);

        string hiddenUserId = Unique(prefix: "hidden-user");

        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        User hiddenUser = new()
        {
            Id = hiddenUserId,
            DefaultCultureId = string.Empty,
            DisplayName = "Hidden User",
            Email = $"{hiddenUserId}@example.com",
            IsActive = true,
        };

        await core.AddUserAsync(user: hiddenUser);

        // When
        User actualUser = await GetUserAsync(id: hiddenUserId);

        // Then
        actualUser.Should()
            .BeNull();

        await Teardown(seededContext: seededContext);
    }
}