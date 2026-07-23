// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            variableName: "CCODER_ACCEPTANCE_CORE_CONNECTION_STRING");

        string ssoConnectionString = GetRequiredConnectionString(
            variableName: "CCODER_ACCEPTANCE_SSO_CONNECTION_STRING");

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

    private static string GetRequiredConnectionString(string variableName) =>
        Environment.GetEnvironmentVariable(variable: variableName)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.User)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.Machine)
        ?? throw new InvalidOperationException(
            message: $"The required {variableName} environment variable is not configured.");

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