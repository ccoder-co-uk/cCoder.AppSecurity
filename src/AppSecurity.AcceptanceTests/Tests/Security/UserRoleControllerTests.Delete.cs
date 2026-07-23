// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class UserRoleControllerTests
{
    [Fact]
    public async Task Delete_RemovesUserRole()
    {
        // Given
        SeededUserRoleContext seededContext = await SeedDatabase();

        await CreateUserRoleAsync(payload: new
        {
            userId = seededContext.UserId,
            roleId = seededContext.GuestRoleId,
        });

        UserRole actualUserRole;

        // When
        int actualStatusCode = await DeleteUserRoleAsync(roleId: seededContext.GuestRoleId, userId: seededContext.UserId);
        actualUserRole = await FindUserRoleAsync(userId: seededContext.UserId, roleId: seededContext.GuestRoleId);

        // Then
        actualStatusCode.Should()
            .Be(expected: 200);

        actualUserRole.Should()
            .BeNull();

        await Teardown(seededContext: seededContext);
    }
}