// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.Data;
using cCoder.Security;
using cCoder.Security.Data.EF;
using AppSecurity.Web.Models;

namespace AppSecurity.Web;

public static class IServiceCollectionExtensions
{
    public static void AddWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfiguration> configure = null)
    {
        AppConfiguration applicationConfiguration = new();
        configuration.Bind(applicationConfiguration);
        configure?.Invoke(applicationConfiguration);

        services.AddData(applicationConfiguration.CoreData);
        services.AddSecurityData(applicationConfiguration.SecurityData);
        services.AddSecurityWeb(applicationConfiguration.Security);
        services.AddAppSecurityWeb(applicationConfiguration.AppSecurity);
    }
}