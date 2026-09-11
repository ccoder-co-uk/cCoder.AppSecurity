// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Models.Entities;

namespace cCoder.AppSecurity.Brokers.Security;

internal interface ISecurityDbContextBroker
{
    SecurityDbContext CreateSecurityDbContext();
    DateTime[] SelectUserEventDates();
    string[] SelectTenantIds();
    TenantAnalysis SelectTenantAnalysis(string tenantId, DateTime createdOn);
    UserActivity[] SelectUserActivities(string tenantId, DateTime from, DateTime to);
    ValueTask AddTenantAnalysesAsync(TenantAnalysis[] newTenantAnalyses, CancellationToken cancellationToken);
    ValueTask DeleteUserEventsBeforeAsync(DateTime createdBefore, CancellationToken cancellationToken);
}