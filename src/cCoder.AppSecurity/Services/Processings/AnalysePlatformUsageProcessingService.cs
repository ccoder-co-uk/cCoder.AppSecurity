// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Brokers.Security;
using cCoder.Security.Models.Entities;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class AnalysePlatformUsageProcessingService(
    ISecurityDbContextBroker securityDbContextBroker,
    IAnalysePlatformUsageService analysePlatformUsageService)
    : IAnalysePlatformUsageProcessingService
{
    public Task RunAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async Task () =>
        {
            ValidateRun(
                cancellationToken: cancellationToken);

            List<DateTime> datesWithData =
                securityDbContextBroker.SelectUserEventDates()
                    .ToList();

            if (datesWithData.FirstOrDefault() == DateTime.Today)
            {
                datesWithData.RemoveAt(index: 0);
            }

            string[] tenants = securityDbContextBroker.SelectTenantIds();

            foreach (DateTime date in datesWithData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                TenantAnalysis[] reports = GenerateDailyReports(
                    tenants: tenants,
                    forDate: date)
                    .ToArray();

                await securityDbContextBroker.AddTenantAnalysesAsync(
                    newTenantAnalyses: reports,
                    cancellationToken: cancellationToken);
            }

            await securityDbContextBroker.DeleteUserEventsBeforeAsync(
                createdBefore: DateTime.Today.AddDays(value: -2),
                cancellationToken: cancellationToken);

        });

    private IEnumerable<TenantAnalysis> GenerateDailyReports(
        string[] tenants,
        DateTime forDate)
    {
        List<TenantAnalysis> results = [];

        foreach (string tenant in tenants)
        {
            results.AddRange(collection: GenerateUserActivityReport(
                tenant: tenant,
                forDate: forDate));
        }

        return results;
    }

    private IEnumerable<TenantAnalysis> GenerateUserActivityReport(
        string tenant,
        DateTime forDate)
    {
        List<TenantAnalysis> results = [];

        TenantAnalysis existingReport =
            securityDbContextBroker.SelectTenantAnalysis(
                tenantId: tenant,
                createdOn: forDate);

        if (existingReport == null)
        {
            results.Add(item: new TenantAnalysis
            {
                TenantId = tenant,
                Key = "System",
                Name = "User Activity (Daily)",
                Value = analysePlatformUsageService.Serialize(
                    value: AnalyseTenantUserActivity(
                        tenantId: tenant,
                        reportDate: forDate)),
                CreatedOn = forDate
            });
        }

        return results;
    }

    private object AnalyseTenantUserActivity(string tenantId, DateTime reportDate)
    {
        UserActivity[] activityData = securityDbContextBroker.SelectUserActivities(
            tenantId: tenantId,
            from: reportDate,
            to: reportDate.AddDays(value: 1));

        return new
        {
            Users = AnalyseUserActivity(data: activityData),
            Pages = AnalysePageActivity(data: activityData),
            ApiCalls = AnalyseApiActivity(data: activityData)
        };
    }

    private static object AnalyseUserActivity(IEnumerable<UserActivity> data) =>
        data
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
                .Select(selector: activity => activity.SessionId)
                .Distinct()
                .Count(),
            PageRequests = group
                .Count(predicate: activity => activity.EventName.StartsWith(value: "Page_GET/") && !activity.EventName.StartsWith(value: "Page_GET/lib/")),
            ApiRequests = group
                .Count(predicate: activity => activity.EventName.StartsWith(value: "Api_GET/"))
        })
        .OrderByDescending(keySelector: item => item.PageRequests + item.ApiRequests)
        .Take(count: 10);

    private static object AnalysePageActivity(IEnumerable<UserActivity> data) =>
        data
        .Where(predicate: activity => activity.EventName.StartsWith(value: "Page_GET/") && !activity.EventName.StartsWith(value: "Page_GET/lib/"))
        .GroupBy(keySelector: activity => activity.EventValue.Split(separator: '?')
            .First())
        .Select(selector: group => new
        {
            Page = group.Key,
            Sessions = group
                .Select(selector: activity => activity.SessionId)
                .Distinct()
                .Count(),
            Hits = group.Count()
        })
        .OrderByDescending(keySelector: item => item.Hits)
        .Take(count: 10);

    private static object AnalyseApiActivity(IEnumerable<UserActivity> data) =>
        data
        .Where(predicate: activity => activity.EventName.StartsWith(value: "Api_"))
        .GroupBy(keySelector: activity => activity.EventValue.Split(separator: '?')
            .First())
        .Select(selector: group => new
        {
            Endpoint = group.Key,
            Sessions = group
                .Select(selector: activity => activity.SessionId)
                .Distinct()
                .Count(),
            Hits = group.Count()
        })
        .OrderByDescending(keySelector: item => item.Hits)
        .Take(count: 10);
}