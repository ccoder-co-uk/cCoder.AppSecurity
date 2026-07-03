using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AppSecurity.HostedServices.AcceptanceTests.Infrastructure;

public sealed class HostedServicesAcceptanceFixture : IAsyncLifetime
{
    public WebApplicationFactory<global::AppSecurity.HostedServices.Program> Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public Task InitializeAsync()
    {
        Factory = new WebApplicationFactory<global::AppSecurity.HostedServices.Program>()
            .WithWebHostBuilder(builder => builder.ConfigureAppConfiguration(
                (_, configuration) => configuration.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["ConnectionStrings:Core"] =
                            "Data Source=.;Initial Catalog=AppSecurityHostedAcceptanceCore;Trusted_Connection=True;Trust Server Certificate=true",
                        ["ConnectionStrings:SSO"] =
                            "Data Source=.;Initial Catalog=AppSecurityHostedAcceptanceSSO;Trusted_Connection=True;Trust Server Certificate=true",
                        ["Settings:DecryptionKey"] = "000000000000000000000000000000000000000000000000",
                        ["Settings:enableExternalEventing"] = "false",
                        ["AppSecurity:IsMigrating"] = "true",
                    })));
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
        });

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (Factory is not null)
            await Factory.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class HostedServicesAcceptanceCollection
    : ICollectionFixture<HostedServicesAcceptanceFixture>
{
    public const string Name = "Hosted services acceptance";
}
