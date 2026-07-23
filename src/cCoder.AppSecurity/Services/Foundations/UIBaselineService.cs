// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class UIBaselineService(IUIBaselineBroker baselineBroker)
    : IUIBaselineService
{
    public Package[] GetPackages() =>
        TryCatch(operation: () =>
        {
            ValidatePackagesOnGet();

            return baselineBroker.SelectPackages();
        });
}