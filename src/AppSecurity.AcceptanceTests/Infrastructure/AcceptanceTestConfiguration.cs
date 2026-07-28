// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;
using Web.AcceptanceTests.Models;


namespace Web.AcceptanceTests.Infrastructure;

internal static class AcceptanceTestConfiguration
{
    public static AcceptanceSettings Create()
    {
        string runId = Guid.NewGuid()
            .ToString(format: "N");

        return new AcceptanceSettings
        {
            CoreConnectionString = CreateAcceptanceConnectionString(
                variableName: "AppSecurity__ConnectionString",
                runId: runId),
            SsoConnectionString = CreateAcceptanceConnectionString(
                variableName: "Security__ConnectionString",
                runId: runId),
            DecryptionKey = GetRequiredValue(
                variableName: "Security__DecryptionKey"),
        };
    }

    private static string CreateAcceptanceConnectionString(
        string variableName,
        string runId)
    {
        SqlConnectionStringBuilder builder = new(
            connectionString: GetRequiredValue(variableName: variableName))
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

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