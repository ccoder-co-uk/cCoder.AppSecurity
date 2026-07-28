// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Security.Objects;

namespace AppSecurity.Web.Models;

public sealed class AppSecurityWebConfiguration
{
    public AppSecurityWebConfiguration()
    {
        AppSecurity = new AppSecurityConfiguration();
        Security = new SecurityConfiguration();
    }

    public AppSecurityConfiguration AppSecurity { get; set; }

    public SecurityConfiguration Security { get; set; }
}