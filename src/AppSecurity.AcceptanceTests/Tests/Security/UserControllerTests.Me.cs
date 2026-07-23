// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class UserControllerTests
{
    [Fact]
    public async Task Me_ReturnsCurrentUser()
    {
        // Given

        // When
        User actualUser = await GetCurrentUserAsync();

        // Then
        actualUser.Should().NotBeNull();
        actualUser!.Id.Should().NotBeNullOrWhiteSpace();
    }
}