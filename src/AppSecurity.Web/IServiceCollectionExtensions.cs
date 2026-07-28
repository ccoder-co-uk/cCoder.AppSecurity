// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.Security;
using AppSecurity.Web.Models;

namespace AppSecurity.Web;

public static class IServiceCollectionExtensions
{
    public static void AddAppSecurityWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppSecurityWebConfiguration> configure = null)
    {
        AppSecurityWebConfiguration applicationConfiguration = new();
        configuration.Bind(applicationConfiguration);
        configure?.Invoke(applicationConfiguration);

        services.AddSecurityWeb(applicationConfiguration.Security);
        services.AddAppSecurityWeb(applicationConfiguration.AppSecurity);
    }
}