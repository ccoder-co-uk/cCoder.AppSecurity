// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.Security;
using cCoder.Security.Data.EF;

namespace AppSecurity.HostedServices;

public static class IServiceCollectionExtensions
{
    public static void AddHostedServicesApplication(
        this IServiceCollection services,
        IConfiguration configuration,
        ILoggingBuilder loggingBuilder)
    {
        string coreConnection =
            configuration.GetConnectionString("Core")
            ?? throw new InvalidOperationException(
                message: "ConnectionStrings:Core is required.");

        string ssoConnection =
            configuration.GetConnectionString("SSO")
            ?? throw new InvalidOperationException(
                message: "ConnectionStrings:SSO is required.");

        services.AddSecurity(configAction: (securityServices, securityConfiguration) =>
        {
            securityConfiguration.AddMSSQLModelProvider(
                services: securityServices,
                connectionString: ssoConnection);

            securityConfiguration.UseAESHMMACPasswordEncryption(
                services: securityServices,
                decryptionKey: configuration.GetSection("Settings")["DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            services: services,
            connectionString: coreConnection);

        services.AddAppSecurityHostedServices(configure: appSecurityConfiguration =>
        {
            appSecurityConfiguration.IsMigrating =
                configuration.GetValue<int?>("MIGRATING") == 1
                || configuration.GetValue<bool?>("AppSecurity:IsMigrating") == true;
        });

        loggingBuilder.ClearProviders();
        loggingBuilder.AddSimpleConsole();
    }
}
