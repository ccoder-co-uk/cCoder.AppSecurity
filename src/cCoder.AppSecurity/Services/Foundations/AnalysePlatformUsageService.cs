// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.AppSecurity.Brokers.Security;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AnalysePlatformUsageService(
    ISecurityDbContextBroker securityDbContextBroker)
    : IAnalysePlatformUsageService
{
    public SecurityDbContext CreateSecurityDbContext() =>
        TryCatch(operation: () =>
        {
            ValidateSecurityDbContextOnCreate();

            return securityDbContextBroker.CreateSecurityDbContext();
        });
}