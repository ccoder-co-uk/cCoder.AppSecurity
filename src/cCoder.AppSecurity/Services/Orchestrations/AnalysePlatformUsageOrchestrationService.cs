// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;


namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed class AnalysePlatformUsageOrchestrationService(
    ISecurityDbContextFactory ssoDbFactory)
    : IAnalysePlatformUsageOrchestrationService
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        using var sso = ssoDbFactory.CreateDbContext();

        List<DateTime> datesWithData = sso.UserEvents
            .IgnoreQueryFilters()
            .Select(selector: userEvent => userEvent.CreatedOn.Date)
            .Distinct()
            .OrderByDescending(keySelector: date => date)
            .ToList();

        if (datesWithData.FirstOrDefault() == DateTime.Today)
        {
            datesWithData.RemoveAt(index: 0);
        }

        string[] tenants = sso.Tenants
            .IgnoreQueryFilters()
            .Select(selector: tenant => tenant.Id)
            .ToArray();

        foreach (DateTime date in datesWithData)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<TenantAnalysis> reports = GenerateDailyReports(tenants: tenants, forDate: date, sso: sso);
            sso.AddRange(entities: reports);
            await sso.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        string sql = $"DELETE UserEvents WHERE CreatedOn < '{DateTime.Today.AddDays(value: -2):yyyy-MM-dd}'";
        sso.Database.ExecuteSqlRaw(sql: sql);
    }

    private static IEnumerable<TenantAnalysis> GenerateDailyReports(string[] tenants, DateTime forDate, SecurityDbContext sso)
    {
        List<TenantAnalysis> results = [];

        foreach (string tenant in tenants)
        {
            results.AddRange(collection: GenerateUserActivityReport(tenant, forDate, sso));
        }

        return results;
    }

    private static IEnumerable<TenantAnalysis> GenerateUserActivityReport(string tenant, DateTime forDate, SecurityDbContext sso)
    {
        List<TenantAnalysis> results = [];

        TenantAnalysis existingReport = sso.TenantAnalysis
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: analysis =>
                analysis.TenantId == tenant &&
                analysis.CreatedOn == forDate &&
                analysis.Name == "User Activity (Daily)");

        if (existingReport == null)
        {
            results.Add(item: new TenantAnalysis
            {
                TenantId = tenant,
                Key = "System",
                Name = "User Activity (Daily)",
                Value = AnalyseTenantUserActivity(tenant, forDate, sso).ToJsonForOdata(),
                CreatedOn = forDate
            });
        }

        return results;
    }

    private static object AnalyseTenantUserActivity(string tenantId, DateTime reportDate, SecurityDbContext sso)
    {
        UserActivity[] activityData = GetUserActivity(tenantId: tenantId, from: reportDate, to: reportDate.AddDays(1), sso: sso);

        return new
        {
            Users = AnalyseUserActivity(data: activityData),
            Pages = AnalysePageActivity(data: activityData),
            ApiCalls = AnalyseApiActivity(data: activityData)
        };
    }

    private static UserActivity[] GetUserActivity(string tenantId, DateTime from, DateTime to, SecurityDbContext sso) =>
        sso.UserEvents
            .IgnoreQueryFilters()
            .Where(predicate: activity => activity.CreatedOn >= from && activity.CreatedOn <= to && activity.TenantId == tenantId)
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
                SessionId = userEvent.SessionId
            })
            .ToArray();

    private static object AnalyseUserActivity(IEnumerable<UserActivity> data) => data
        .GroupBy(keySelector: activity => activity.UserId)
        .Select(selector: group => new
        {
            User = new
            {
                Id = group.Key,
                group.First().UserEmail,
                group.First().UserDisplayName
            },
            Sessions = group
                .Select(activity => activity.SessionId)
                .Distinct()
                .Count(),
            PageRequests = group
                .Count(activity => activity.EventName.StartsWith("Page_GET/") && !activity.EventName.StartsWith("Page_GET/lib/")),
            ApiRequests = group
                .Count(activity => activity.EventName.StartsWith("Api_GET/"))
        })
        .OrderByDescending(keySelector: item => item.PageRequests + item.ApiRequests)
        .Take(count: 10);

    private static object AnalysePageActivity(IEnumerable<UserActivity> data) => data
        .Where(predicate: activity => activity.EventName.StartsWith("Page_GET/") && !activity.EventName.StartsWith("Page_GET/lib/"))
        .GroupBy(keySelector: activity => activity.EventValue.Split('?').First())
        .Select(selector: group => new
        {
            Page = group.Key,
            Sessions = group
                .Select(activity => activity.SessionId)
                .Distinct()
                .Count(),
            Hits = group.Count()
        })
        .OrderByDescending(keySelector: item => item.Hits)
        .Take(count: 10);

    private static object AnalyseApiActivity(IEnumerable<UserActivity> data) => data
        .Where(predicate: activity => activity.EventName.StartsWith("Api_"))
        .GroupBy(keySelector: activity => activity.EventValue.Split('?').First())
        .Select(selector: group => new
        {
            Endpoint = group.Key,
            Sessions = group
                .Select(activity => activity.SessionId)
                .Distinct()
                .Count(),
            Hits = group.Count()
        })
        .OrderByDescending(keySelector: item => item.Hits)
        .Take(count: 10);
}