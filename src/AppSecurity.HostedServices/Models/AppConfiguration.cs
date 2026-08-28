// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models;
using cCoder.Security.Models;

namespace AppSecurity.HostedServices.Models;

public sealed class AppConfiguration
{
    public AppSecurityConfiguration AppSecurity { get; set; }

    public CoreDataConfiguration CoreData { get; set; }

    public SecurityConfiguration Security { get; set; }

    public SecurityDataConfiguration SecurityData { get; set; }
}