// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Models;

public sealed class AppSecurityPackageEvent
{
    public int AppId { get; set; }

    public Package Package { get; set; }
}