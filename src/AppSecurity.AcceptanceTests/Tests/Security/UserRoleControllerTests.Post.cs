// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class UserRoleControllerTests
{
    [Fact]
    public async Task Post_CreatesUserRole()
    {
        // Given
        SeededUserRoleContext seededContext = await SeedDatabase();
        UserRole actualUserRole;

        // When
        await CreateUserRoleAsync(new
        {
            userId = seededContext.UserId,
            roleId = seededContext.GuestRoleId,
        });

        actualUserRole = await FindUserRoleAsync(seededContext.UserId, seededContext.GuestRoleId);

        // Then
        actualUserRole.Should().NotBeNull();
        actualUserRole!.UserId.Should().Be(seededContext.UserId);

        await Teardown(seededContext);
    }

    [Fact]
    public async Task Post_CreatesUserRoleForHiddenUserWhenIdsAreKnown()
    {
        // Given
        SeededUserRoleContext seededContext = await SeedDatabase();

        // When
        await CreateUserRoleAsync(new
        {
            userId = seededContext.HiddenUserId,
            roleId = seededContext.BasicRoleId,
        });

        UserRole actualUserRole = await FindUserRoleAsync(seededContext.HiddenUserId, seededContext.BasicRoleId);

        // Then
        actualUserRole.Should().NotBeNull();
        actualUserRole!.UserId.Should().Be(seededContext.HiddenUserId);

        await Teardown(seededContext);
    }

    [Fact]
    public async Task Post_RejectsAssigningAppAdminRoleWhenActorIsNotAppAdmin()
    {
        // Given
        RestrictedUserRoleContext seededContext = await SeedRestrictedDatabase();

        // When
        (int statusCode, string content) = await PostUserRoleAsync(new
        {
            userId = seededContext.UserId,
            roleId = seededContext.TargetAdminRoleId,
        });

        UserRole actualUserRole = await FindUserRoleAsync(seededContext.UserId, seededContext.TargetAdminRoleId);

        // Then
        statusCode.Should().Be((int)HttpStatusCode.Unauthorized, content);
        content.Should().Contain("Access Denied!");
        actualUserRole.Should().BeNull();

        await Teardown(seededContext);
    }
}