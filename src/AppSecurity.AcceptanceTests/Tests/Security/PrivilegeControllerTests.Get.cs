using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Security;

public sealed partial class PrivilegeControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsUnauthorized()
    {
        // Given

        // When
        int actualStatusCode = await GetPrivilegeStatusCodeAsync($"{BaseUrl}/$count");

        // Then
        actualStatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Get_ReturnsPrivilegeById()
    {
        // Given
        string id = Unique("privilege");
        await CreatePrivilegeAsync(new
        {
            id,
            type = "Acceptance",
            operation = "Execute",
            description = "Acceptance privilege",
            portalAdminsOnly = false,
        });

        // When
        int actualStatusCode = await GetPrivilegeStatusCodeAsync($"{BaseUrl}('{Uri.EscapeDataString(id)}')");

        // Then
        actualStatusCode.Should().Be(404);

        await DeletePrivilegeAsync(id);
    }
}





