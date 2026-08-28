// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.AppSecurity.Models;

public class AppSecurityConfiguration
{
    public bool AggregateDomains { get; set; }
    public string RootPath { get; set; }
    public bool IncludeLegacyCoreContext { get; set; }
    public bool IsMigrating { get; set; }
    public EventProvider[] EventProviders { get; set; }
}