// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.Setup;
using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Brokers;

internal sealed class UIBaselineBroker : IUIBaselineBroker
{
    public Package[] SelectPackages() =>
        UIBaseline.Packages;
}