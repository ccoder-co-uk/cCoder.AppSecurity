// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.AppSecurity.Models;

public class AppSecurityConfiguration
{
    public AppSecurityConfiguration()
    {
        ConnectionStrings = new Dictionary<string, string>();
        Settings = new Dictionary<string, string>();
        Services = new Dictionary<string, string>();
        RootPath = "Api/AppSecurity";
        IncludeLegacyCoreContext = true;
        EventProviders = [];
    }

    public IDictionary<string, string> ConnectionStrings { get; set; }
    public IDictionary<string, string> Settings { get; set; }
    public IDictionary<string, string> Services { get; set; }
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; }
    public bool IncludeLegacyCoreContext { get; set; }
    public bool IsMigrating { get; set; }
    public EventProvider[] EventProviders { get; set; }
}