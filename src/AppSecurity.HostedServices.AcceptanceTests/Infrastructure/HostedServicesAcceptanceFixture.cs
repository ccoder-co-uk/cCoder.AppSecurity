// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;
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
        string coreConnectionString = GetRequiredConnectionString(
            variableName: "ConnectionStrings__Core");

        string ssoConnectionString = GetRequiredConnectionString(
            variableName: "ConnectionStrings__SSO");

        Factory = new WebApplicationFactory<global::AppSecurity.HostedServices.Program>()
            .WithWebHostBuilder(configuration: builder => builder.ConfigureAppConfiguration(
                configureDelegate: (_, configuration) => configuration.AddInMemoryCollection(
                    initialData: new Dictionary<string, string>
                    {
                        ["ConnectionStrings:Core"] = coreConnectionString,
                        ["ConnectionStrings:SSO"] = ssoConnectionString,
                        ["Settings:DecryptionKey"] = "000000000000000000000000000000000000000000000000",
                        ["Settings:enableExternalEventing"] = "false",
                        ["AppSecurity:IsMigrating"] = "true",
                    })));

        Client = Factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });

        return Task.CompletedTask;
    }

    private static string GetRequiredConnectionString(string variableName)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(variable: variableName)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.Machine)
            ?? throw new InvalidOperationException(
                message: $"The required {variableName} environment variable is not configured.");

        SqlConnectionStringBuilder builder = new(connectionString);
        builder.InitialCatalog = $"{builder.InitialCatalog}-acceptance-{Guid.NewGuid():N}";
        return builder.ConnectionString;
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