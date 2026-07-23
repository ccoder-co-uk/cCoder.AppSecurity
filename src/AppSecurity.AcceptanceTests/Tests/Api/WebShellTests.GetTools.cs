// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        actualContent.Should()
            .Contain(expected: "App Security");

        actualContent.Should()
            .Contain(expected: "Application roles, users, and privilege assignments");

        actualContent.Should()
            .Contain(expected: "/tools/company-logo.png");

        actualContent.Should()
            .Contain(expected: "as-logo");

        actualContent.Should()
            .Contain(expected: "Sign in required");

        actualContent.Should()
            .Contain(expected: "as-login-gate");

        actualContent.Should()
            .Contain(expected: "as-workbench");

        actualContent.Should()
            .Contain(expected: "App Security workspace tabs");

        actualContent.Should()
            .Contain(expected: "/tools/api.js");

        actualContent.Should()
            .Contain(expected: "/tools/grids.js");

        actualContent.Should()
            .Contain(expected: "/tools/styles.css?v=appsecurity-tools-20260702-auth");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsWorkbenchScripts()
    {
        // Given

        // When
        string actualApiScript = await GetScriptAsync(scriptName: "api.js");
        string actualGridScript = await GetScriptAsync(scriptName: "grids.js");

        // Then
        actualApiScript.Should()
            .Contain(expected: "window.AppSecurityApi");

        actualApiScript.Should()
            .Contain(expected: "appsecurity-auth-changed");

        actualApiScript.Should()
            .Contain(expected: "isAuthenticated: function");

        actualApiScript.Should()
            .Contain(expected: "document.body.classList.toggle(\"is-authenticated\"");

        actualGridScript.Should()
            .Contain(expected: "window.AppSecurityGrids");

        actualGridScript.Should()
            .Contain(expected: "AppSecurityApi.isAuthenticated()");

        actualGridScript.Should()
            .Contain(expected: "appsecurity-auth-changed");

        actualGridScript.Should()
            .Contain(expected: "Add user");

        actualGridScript.Should()
            .Contain(expected: "Save privileges");

        actualGridScript.Should()
            .Contain(expected: "/Privilege?$top=500");

        actualGridScript.Should()
            .NotContain(unexpected: "title: \"Privileges\"");

        actualGridScript.Should()
            .NotContain(unexpected: "title: \"Role Privileges\"");

        actualGridScript.Should()
            .NotContain(unexpected: "title: \"User Roles\"");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsLoginGateAndTabStyles()
    {
        // Given

        // When
        string actualStyles = await GetScriptAsync(scriptName: "styles.css");

        // Then
        actualStyles.Should()
            .Contain(expected: ".as-logo");

        actualStyles.Should()
            .Contain(expected: "body.as-shell:not(.is-authenticated) .as-workbench");

        actualStyles.Should()
            .Contain(expected: "body.as-shell.is-authenticated .as-login-gate");

        actualStyles.Should()
            .Contain(expected: "grid-template-rows: auto minmax(0, 1fr)");

        actualStyles.Should()
            .Contain(expected: ".as-nav-item.active");

        actualStyles.Should()
            .Contain(expected: "border-radius: 4px 4px 0 0");
    }
}