// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.Data;
using cCoder.Security;
using cCoder.Security.Data.EF;
using AppSecurity.HostedServices.Models;

namespace AppSecurity.HostedServices;

public static class IServiceCollectionExtensions
{
    public static void AddHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfiguration> configure = null)
    {
        AppConfiguration applicationConfiguration = new();
        configuration.Bind(applicationConfiguration);
        configure?.Invoke(applicationConfiguration);

        services.AddData(applicationConfiguration.CoreData);
        services.AddSecurityData(applicationConfiguration.SecurityData);
        services.AddSecurityHostedServices(
            applicationConfiguration.Security);
        services.AddAppSecurityHostedServices(
            applicationConfiguration.AppSecurity);
    }
}