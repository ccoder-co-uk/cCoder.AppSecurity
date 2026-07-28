// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using AppSecurity.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.AcceptanceTests.Models;


namespace Web.AcceptanceTests.Infrastructure;

internal sealed class WebAcceptanceFactory(AcceptanceSettings settings)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment: "Acceptance");

        builder.UseSetting(
            key: "AppSecurity:ConnectionString",
            value: settings.CoreConnectionString);

        builder.UseSetting(
            key: "Security:ConnectionString",
            value: settings.SsoConnectionString);

        builder.UseSetting(
            key: "Security:DecryptionKey",
            value: settings.DecryptionKey);
    }
}