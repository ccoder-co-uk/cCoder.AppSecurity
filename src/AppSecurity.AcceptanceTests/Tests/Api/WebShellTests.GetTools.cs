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
        actualContent.Should().Contain("/tools/company-logo.png");
        actualContent.Should().Contain("as-logo");
        actualContent.Should().Contain("Sign in required");
        actualContent.Should().Contain("as-login-gate");
        actualContent.Should().Contain("as-workbench");
        actualContent.Should().Contain("App Security workspace tabs");
        actualContent.Should().Contain("/tools/api.js");
        actualContent.Should().Contain("/tools/grids.js");
        actualContent.Should().Contain("/tools/styles.css?v=appsecurity-tools-20260702-auth");
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
        actualApiScript.Should().Contain("appsecurity-auth-changed");
        actualApiScript.Should().Contain("isAuthenticated: function");
        actualApiScript.Should().Contain("document.body.classList.toggle(\"is-authenticated\"");
        actualGridScript.Should().Contain("window.AppSecurityGrids");
        actualGridScript.Should().Contain("AppSecurityApi.isAuthenticated()");
        actualGridScript.Should().Contain("appsecurity-auth-changed");
        actualGridScript.Should().Contain("Add user");
        actualGridScript.Should().Contain("Save privileges");
        actualGridScript.Should().Contain("/Privilege?$top=500");
        actualGridScript.Should().NotContain("title: \"Privileges\"");
        actualGridScript.Should().NotContain("title: \"Role Privileges\"");
        actualGridScript.Should().NotContain("title: \"User Roles\"");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsLoginGateAndTabStyles()
    {
        // Given

        // When
        string actualStyles = await GetScriptAsync("styles.css");

        // Then
        actualStyles.Should().Contain(".as-logo");
        actualStyles.Should().Contain("body.as-shell:not(.is-authenticated) .as-workbench");
        actualStyles.Should().Contain("body.as-shell.is-authenticated .as-login-gate");
        actualStyles.Should().Contain("grid-template-rows: auto minmax(0, 1fr)");
        actualStyles.Should().Contain(".as-nav-item.active");
        actualStyles.Should().Contain("border-radius: 4px 4px 0 0");
    }
}
