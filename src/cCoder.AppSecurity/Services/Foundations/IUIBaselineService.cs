// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.AppSecurity.Services.Foundations;

public interface IUIBaselineService
{
    Package[] GetPackages();
}