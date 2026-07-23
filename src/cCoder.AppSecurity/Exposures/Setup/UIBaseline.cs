// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Exposures.Setup;

public static partial class UIBaseline
{
    public static Package[] Packages => [
        Components,
        Pages,
        Resources,
        Templates,
        Roles,
        PageRoles
    ];
}