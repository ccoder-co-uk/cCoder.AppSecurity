// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using AppSecurity.Web;
using cCoder.AppSecurity;
using cCoder.Data;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Models;


namespace Web.AcceptanceTests.Infrastructure;

internal sealed class WebAcceptanceFactory(AcceptanceSettings settings)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment: "Acceptance");

        builder.ConfigureServices(configureServices: services =>
        {
            services.AddSingleton(
                implementationInstance: new cCoder.Data.Config
                {
                    ConnectionStrings = new Dictionary<string, string>
                    {
                        ["Core"] = settings.CoreConnectionString,
                        ["SSO"] = settings.SsoConnectionString,
                    },
                    Settings = new Dictionary<string, string>
                    {
                        ["DecryptionKey"] = settings.DecryptionKey,
                        ["enableExternalEventing"] = "false",
                    },
                    Services = new Dictionary<string, string>(),
                });

            services.AddSingleton<ISecurityDbContextFactory>(
                implementationFactory: _ =>
                    new MSSQLSecurityDbContextFactory(
                        connectionString: settings.SsoConnectionString));

            services.AddCoreData(connectionString: settings.CoreConnectionString);
        });
    }
}