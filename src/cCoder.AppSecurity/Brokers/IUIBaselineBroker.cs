// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Brokers;

internal interface IUIBaselineBroker
{
    Package[] SelectPackages();
}