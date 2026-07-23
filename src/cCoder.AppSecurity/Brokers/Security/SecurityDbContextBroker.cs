// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;

namespace cCoder.AppSecurity.Brokers.Security;

internal sealed class SecurityDbContextBroker(
    ISecurityDbContextFactory securityDbContextFactory)
    : ISecurityDbContextBroker
{
    public SecurityDbContext CreateSecurityDbContext() =>
        securityDbContextFactory.CreateDbContext();
}