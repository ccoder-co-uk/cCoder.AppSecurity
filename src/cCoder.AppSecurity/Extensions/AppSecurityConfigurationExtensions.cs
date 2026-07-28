// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Eventing.Models;

namespace cCoder.AppSecurity.Extensions;

public static class AppSecurityConfigurationExtensions
{
    public static AppSecurityConfiguration WithEventProviders(
        this AppSecurityConfiguration configuration,
        params EventProvider[] eventProviders)
    {
        configuration.EventProviders = eventProviders ?? [];

        return configuration;
    }
}