// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Models;

internal sealed class AppSecurityPackageMapping
{
    public int AppId { get; set; }
    public AppSecurityPackage Package { get; set; }
    public App App { get; set; }
}