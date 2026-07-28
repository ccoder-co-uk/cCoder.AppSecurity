// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.Security;
using AppSecurity.HostedServices.Models;

namespace AppSecurity.HostedServices;

public static class IServiceCollectionExtensions
{
    public static void AddAppSecurityHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppSecurityHostedServicesConfiguration> configure = null)
    {
        AppSecurityHostedServicesConfiguration applicationConfiguration = new();
        configuration.Bind(applicationConfiguration);
        configure?.Invoke(applicationConfiguration);

        services.AddSecurityHostedServices(
            applicationConfiguration.Security);
        services.AddAppSecurityHostedServices(
            applicationConfiguration.AppSecurity);
    }
}