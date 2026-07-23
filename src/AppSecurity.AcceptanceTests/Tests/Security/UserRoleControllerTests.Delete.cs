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
        await CreateUserRoleAsync(new
        {
            userId = seededContext.UserId,
            roleId = seededContext.GuestRoleId,
        });
        UserRole actualUserRole;

        // When
        int actualStatusCode = await DeleteUserRoleAsync(seededContext.GuestRoleId, seededContext.UserId);
        actualUserRole = await FindUserRoleAsync(seededContext.UserId, seededContext.GuestRoleId);

        // Then
        actualStatusCode.Should().Be(200);
        actualUserRole.Should().BeNull();

        await Teardown(seededContext);
    }
}