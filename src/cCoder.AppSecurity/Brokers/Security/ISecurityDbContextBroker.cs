// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;

namespace cCoder.AppSecurity.Brokers.Security;

internal interface ISecurityDbContextBroker
{
    SecurityDbContext CreateSecurityDbContext();
}