// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;


namespace AppSecurity.HostedServices.AcceptanceTests.Infrastructure;

internal sealed class AcceptanceTestConfiguration
{
    private AcceptanceTestConfiguration(
        string appSecurityConnectionString,
        string securityConnectionString,
        string securityDecryptionKey)
    {
        AppSecurityConnectionString = appSecurityConnectionString;
        SecurityConnectionString = securityConnectionString;
        SecurityDecryptionKey = securityDecryptionKey;
    }

    public string AppSecurityConnectionString { get; }

    public string SecurityConnectionString { get; }

    public string SecurityDecryptionKey { get; }

    public static AcceptanceTestConfiguration Create()
    {
        string runId = Guid.NewGuid()
            .ToString(format: "N");

        return new AcceptanceTestConfiguration(
            appSecurityConnectionString: CreateAcceptanceConnectionString(
                variableName: "AppSecurity__ConnectionString",
                runId: runId),
            securityConnectionString: CreateAcceptanceConnectionString(
                variableName: "Security__ConnectionString",
                runId: runId),
            securityDecryptionKey: GetRequiredValue(
                variableName: "Security__DecryptionKey"));
    }

    private static string CreateAcceptanceConnectionString(
        string variableName,
        string runId)
    {
        SqlConnectionStringBuilder builder = new(
            connectionString: GetRequiredValue(variableName: variableName));

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            throw new InvalidOperationException(
                message: $"{variableName} must define a database name.");
        }

        builder.InitialCatalog =
            $"{builder.InitialCatalog}-acceptance-{runId}";

        return builder.ConnectionString;
    }

    private static string GetRequiredValue(string variableName) =>
        Environment.GetEnvironmentVariable(variable: variableName)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.User)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.Machine)
        ?? throw new InvalidOperationException(
            message: $"The required {variableName} environment variable is not configured.");
}