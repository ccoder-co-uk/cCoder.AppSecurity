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
            .Select(userEvent => userEvent.CreatedOn.Date)
            .Distinct()
            .OrderByDescending(date => date)
            .ToList();

        if (datesWithData.FirstOrDefault() == DateTime.Today)
            datesWithData.RemoveAt(0);

        string[] tenants = sso.Tenants
            .IgnoreQueryFilters()
            .Select(tenant => tenant.Id)
            .ToArray();

        foreach (DateTime date in datesWithData)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<TenantAnalysis> reports = GenerateDailyReports(tenants, date, sso);
            sso.AddRange(reports);
            await sso.SaveChangesAsync(cancellationToken);
        }

        string sql = $"DELETE UserEvents WHERE CreatedOn < '{DateTime.Today.AddDays(-2):yyyy-MM-dd}'";
        sso.Database.ExecuteSqlRaw(sql);
    }

    private static IEnumerable<TenantAnalysis> GenerateDailyReports(string[] tenants, DateTime forDate, SecurityDbContext sso)
    {
        List<TenantAnalysis> results = [];

        foreach (string tenant in tenants)
            results.AddRange(GenerateUserActivityReport(tenant, forDate, sso));

        return results;
    }

    private static IEnumerable<TenantAnalysis> GenerateUserActivityReport(string tenant, DateTime forDate, SecurityDbContext sso)
    {
        List<TenantAnalysis> results = [];

        TenantAnalysis existingReport = sso.TenantAnalysis
            .IgnoreQueryFilters()
            .FirstOrDefault(analysis =>
                analysis.TenantId == tenant &&
                analysis.CreatedOn == forDate &&
                analysis.Name == "User Activity (Daily)");

        if (existingReport == null)
        {
            results.Add(new TenantAnalysis
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
        UserActivity[] activityData = GetUserActivity(tenantId, reportDate, reportDate.AddDays(1), sso);

        return new
        {
            Users = AnalyseUserActivity(activityData),
            Pages = AnalysePageActivity(activityData),
            ApiCalls = AnalyseApiActivity(activityData)
        };
    }

    private static UserActivity[] GetUserActivity(string tenantId, DateTime from, DateTime to, SecurityDbContext sso) =>
        sso.UserEvents
            .IgnoreQueryFilters()
            .Where(activity => activity.CreatedOn >= from && activity.CreatedOn <= to && activity.TenantId == tenantId)
            .Select(userEvent => new UserActivity
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
        .GroupBy(activity => activity.UserId)
        .Select(group => new
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
        .OrderByDescending(item => item.PageRequests + item.ApiRequests)
        .Take(10);

    private static object AnalysePageActivity(IEnumerable<UserActivity> data) => data
        .Where(activity => activity.EventName.StartsWith("Page_GET/") && !activity.EventName.StartsWith("Page_GET/lib/"))
        .GroupBy(activity => activity.EventValue.Split('?').First())
        .Select(group => new
        {
            Page = group.Key,
            Sessions = group
                .Select(activity => activity.SessionId)
                .Distinct()
                .Count(),
            Hits = group.Count()
        })
        .OrderByDescending(item => item.Hits)
        .Take(10);

    private static object AnalyseApiActivity(IEnumerable<UserActivity> data) => data
        .Where(activity => activity.EventName.StartsWith("Api_"))
        .GroupBy(activity => activity.EventValue.Split('?').First())
        .Select(group => new
        {
            Endpoint = group.Key,
            Sessions = group
                .Select(activity => activity.SessionId)
                .Distinct()
                .Count(),
            Hits = group.Count()
        })
        .OrderByDescending(item => item.Hits)
        .Take(10);
}

