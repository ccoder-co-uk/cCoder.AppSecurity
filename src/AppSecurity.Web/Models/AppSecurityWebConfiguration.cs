// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Security.Models;

namespace AppSecurity.Web.Models;

public sealed class AppSecurityWebConfiguration
{
    public AppSecurityConfiguration AppSecurity { get; set; }

    public SecurityConfiguration Security { get; set; }
}