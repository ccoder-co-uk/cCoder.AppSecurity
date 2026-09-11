// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.AppSecurity.Brokers.Security;

internal sealed class SecurityDbContextBroker(
    ISecurityDbContextFactory securityDbContextFactory)
    : ISecurityDbContextBroker
{
    public SecurityDbContext CreateSecurityDbContext() =>
        securityDbContextFactory.CreateDbContext();

    public DateTime[] SelectUserEventDates()
    {
        using SecurityDbContext securityDbContext = CreateSecurityDbContext();

        return securityDbContext.UserEvents
            .IgnoreQueryFilters()
            .Select(selector: userEvent => userEvent.CreatedOn)
            .Distinct()
            .AsEnumerable()
            .Select(selector: createdOn => createdOn.Date)
            .Distinct()
            .OrderByDescending(keySelector: date => date)
            .ToArray();
    }

    public string[] SelectTenantIds()
    {
        using SecurityDbContext securityDbContext = CreateSecurityDbContext();

        return securityDbContext.Tenants
            .IgnoreQueryFilters()
            .Select(selector: tenant => tenant.Id)
            .ToArray();
    }

    public TenantAnalysis SelectTenantAnalysis(string tenantId, DateTime createdOn)
    {
        using SecurityDbContext securityDbContext = CreateSecurityDbContext();

        return securityDbContext.TenantAnalysis
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: analysis =>
                analysis.TenantId == tenantId
                && analysis.CreatedOn == createdOn
                && analysis.Name == "User Activity (Daily)");
    }

    public UserActivity[] SelectUserActivities(
        string tenantId,
        DateTime from,
        DateTime to)
    {
        using SecurityDbContext securityDbContext = CreateSecurityDbContext();

        return securityDbContext.UserEvents
            .IgnoreQueryFilters()
            .Where(predicate: activity =>
                activity.CreatedOn >= from
                && activity.CreatedOn <= to
                && activity.TenantId == tenantId)
            .Select(selector: userEvent => new UserActivity
            {
                TenantId = userEvent.TenantId,
                TenantName = userEvent.Tenant.Name,
                TenantDescription = userEvent.Tenant.Description,
                TenantCreatedBy = userEvent.Tenant.CreatedBy,
                TenantLastUpdatedBy = userEvent.Tenant.LastUpdatedBy,
                TenantCreatedOn = userEvent.Tenant.CreatedOn,
                TenantLastUpdated = userEvent.Tenant.LastUpdated,
                UserId = userEvent.CreatedBy,
                UserDisplayName = userEvent.CreatedByUser.DisplayName,
                UserEmail = userEvent.CreatedByUser.Email,
                UserPhoneNumber = userEvent.TenantId,
                EventId = userEvent.Id,
                EventName = userEvent.EventName,
                EventValue = userEvent.Value,
                EventCreatedOn = userEvent.CreatedOn,
                SessionId = userEvent.SessionId,
            })
            .ToArray();
    }

    public async ValueTask AddTenantAnalysesAsync(
        TenantAnalysis[] newTenantAnalyses,
        CancellationToken cancellationToken)
    {
        using SecurityDbContext securityDbContext = CreateSecurityDbContext();
        securityDbContext.AddRange(entities: newTenantAnalyses);
        await securityDbContext.SaveChangesAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask DeleteUserEventsBeforeAsync(
        DateTime createdBefore,
        CancellationToken cancellationToken)
    {
        using SecurityDbContext securityDbContext = CreateSecurityDbContext();

        await securityDbContext.UserEvents
            .IgnoreQueryFilters()
            .Where(predicate: userEvent => userEvent.CreatedOn < createdBefore)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }
}