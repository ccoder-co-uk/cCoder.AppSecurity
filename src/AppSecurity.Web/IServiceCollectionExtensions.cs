// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Apps.Shared.Models;
using cCoder.AppSecurity;
using cCoder.Eventing;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Objects;

namespace AppSecurity.Web;

public static class IServiceCollectionExtensions
{
    public static void AddAppSecurityWebApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string coreConnection =
            configuration.GetConnectionString("Core")
            ?? throw new InvalidOperationException(
                message: "ConnectionStrings:Core is required.");

        string ssoConnection =
            configuration.GetConnectionString("SSO")
            ?? throw new InvalidOperationException(
                message: "ConnectionStrings:SSO is required.");

        Config config = new();
        configuration.Bind(config);
        services.AddSingleton(implementationInstance: config);
        services.AddEventing();

        services.AddSecurityApi(configAction: (securityServices, securityConfiguration) =>
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

        services.AddAppSecurityWeb();
    }
}