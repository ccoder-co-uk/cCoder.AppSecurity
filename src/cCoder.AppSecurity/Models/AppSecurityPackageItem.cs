// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.AppSecurity.Models;

public class AppSecurityPackageItem
{
    public Guid Id { get; set; }

    public Guid PackageId { get; set; }

    public string Type { get; set; }

    public string Data { get; set; }

    public virtual AppSecurityPackage Package { get; set; }
}