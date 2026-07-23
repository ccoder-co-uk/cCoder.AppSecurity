// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;

namespace cCoder.AppSecurity.Services.Foundations;

internal interface IAnalysePlatformUsageService
{
    SecurityDbContext CreateSecurityDbContext();
}