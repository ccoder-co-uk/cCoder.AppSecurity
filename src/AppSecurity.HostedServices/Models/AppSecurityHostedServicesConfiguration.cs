// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Security.Models;

namespace AppSecurity.HostedServices.Models;

public sealed class AppSecurityHostedServicesConfiguration
{
    public AppSecurityHostedServicesConfiguration()
    {
        AppSecurity = new AppSecurityConfiguration();
        Security = new SecurityConfiguration();
    }

    public AppSecurityConfiguration AppSecurity { get; set; }

    public SecurityConfiguration Security { get; set; }
}