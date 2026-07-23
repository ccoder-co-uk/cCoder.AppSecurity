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
        await CreateUserRoleAsync(payload: new
        {
            userId = seededContext.UserId,
            roleId = seededContext.GuestRoleId,
        });

        actualUserRole = await FindUserRoleAsync(userId: seededContext.UserId, roleId: seededContext.GuestRoleId);

        // Then
        actualUserRole.Should()
            .NotBeNull();

        actualUserRole!.UserId.Should()
            .Be(expected: seededContext.UserId);

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Post_CreatesUserRoleForHiddenUserWhenIdsAreKnown()
    {
        // Given
        SeededUserRoleContext seededContext = await SeedDatabase();

        // When
        await CreateUserRoleAsync(payload: new
        {
            userId = seededContext.HiddenUserId,
            roleId = seededContext.BasicRoleId,
        });

        UserRole actualUserRole = await FindUserRoleAsync(userId: seededContext.HiddenUserId, roleId: seededContext.BasicRoleId);

        // Then
        actualUserRole.Should()
            .NotBeNull();

        actualUserRole!.UserId.Should()
            .Be(expected: seededContext.HiddenUserId);

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Post_RejectsAssigningAppAdminRoleWhenActorIsNotAppAdmin()
    {
        // Given
        RestrictedUserRoleContext seededContext = await SeedRestrictedDatabase();

        // When
        (int statusCode, string content) = await PostUserRoleAsync(payload: new
        {
            userId = seededContext.UserId,
            roleId = seededContext.TargetAdminRoleId,
        });

        UserRole actualUserRole = await FindUserRoleAsync(userId: seededContext.UserId, roleId: seededContext.TargetAdminRoleId);

        // Then
        statusCode.Should()
            .Be(expected: (int)HttpStatusCode.Unauthorized, because: content);

        content.Should()
            .Contain(expected: "Access Denied!");

        actualUserRole.Should()
            .BeNull();

        await Teardown(seededContext: seededContext);
    }
}