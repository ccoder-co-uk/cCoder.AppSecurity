// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AppSecurity.HostedServices.AcceptanceTests.Infrastructure;

public sealed class HostedServicesAcceptanceFixture : IAsyncLifetime
{
    public WebApplicationFactory<global::AppSecurity.HostedServices.Program> Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public Task InitializeAsync()
    {
        AcceptanceTestConfiguration settings =
            AcceptanceTestConfiguration.Create();

        Factory = new WebApplicationFactory<global::AppSecurity.HostedServices.Program>()
            .WithWebHostBuilder(configuration: builder =>
            {
                builder.UseSetting(
                    key: "AppSecurity:ConnectionString",
                    value: settings.AppSecurityConnectionString);

                builder.UseSetting(
                    key: "AppSecurity:IsMigrating",
                    value: bool.TrueString);

                builder.UseSetting(
                    key: "Security:ConnectionString",
                    value: settings.SecurityConnectionString);

                builder.UseSetting(
                    key: "Security:DecryptionKey",
                    value: settings.SecurityDecryptionKey);
            });

        Client = Factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
    }
}

[CollectionDefinition(Name)]
public sealed class HostedServicesAcceptanceCollection
    : ICollectionFixture<HostedServicesAcceptanceFixture>
{
    public const string Name = "Hosted services acceptance";
}