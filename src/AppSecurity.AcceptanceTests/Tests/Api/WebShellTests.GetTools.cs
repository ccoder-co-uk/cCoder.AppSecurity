using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task GetTools_ReturnsAppSecurityWorkbench()
    {
        // Given

        // When
        string actualContent = await GetToolsAsync();

        // Then
        actualContent.Should().Contain("App Security");
        actualContent.Should().Contain("Application roles, users, and privilege assignments");
        actualContent.Should().Contain("/tools/api.js");
        actualContent.Should().Contain("/tools/grids.js");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsWorkbenchScripts()
    {
        // Given

        // When
        string actualApiScript = await GetScriptAsync("api.js");
        string actualGridScript = await GetScriptAsync("grids.js");

        // Then
        actualApiScript.Should().Contain("window.AppSecurityApi");
        actualGridScript.Should().Contain("window.AppSecurityGrids");
        actualGridScript.Should().Contain("Privilege assignment");
        actualGridScript.Should().Contain("/Privilege?$top=500");
        actualGridScript.Should().NotContain("title: \"Privileges\"");
        actualGridScript.Should().NotContain("title: \"Role Privileges\"");
    }
}
