// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class AnalysePlatformUsageProcessingServiceTests
{
    [Fact]
    public async Task ShouldGenerateDailyTenantUsageReportOnRunAsync()
    {
        // Given
        DateTime reportDate = DateTime.Today.AddDays(value: -1);

        Tenant tenant = new()
        {
            Id = "tenant-one",
            Name = "Tenant One",
            Description = "Tenant description",
            CreatedBy = "user-one",
            LastUpdatedBy = "user-one",
            CreatedOn = reportDate,
            LastUpdated = reportDate
        };

        SSOUser user = new()
        {
            Id = "user-one",
            DisplayName = "User One",
            Email = "user.one@example.com"
        };

        securityDbContext.AddRange(
            tenant,
            user,
            CreateUserEvent(
                tenant: tenant,
                user: user,
                createdOn: reportDate,
                eventName: "Page_GET/home",
                eventValue: "/home?source=test"),
            CreateUserEvent(
                tenant: tenant,
                user: user,
                createdOn: reportDate,
                eventName: "Api_GET/users",
                eventValue: "/api/users?source=test"),
            CreateUserEvent(
                tenant: tenant,
                user: user,
                createdOn: reportDate,
                eventName: "Page_GET/lib/client.js",
                eventValue: "/lib/client.js"));

        await securityDbContext.SaveChangesAsync();

        // When
        await processingService.RunAsync();

        // Then
        using SecurityDbContext verificationContext = CreateSecurityDbContext();

        TenantAnalysis report = await verificationContext.TenantAnalysis
            .IgnoreQueryFilters()
            .SingleAsync(predicate: analysis =>
                analysis.CreatedOn == reportDate);

        report.TenantId
            .Should()
            .Be(expected: tenant.Id);

        report.Name
            .Should()
            .Be(expected: "User Activity (Daily)");

        report.CreatedOn
            .Should()
            .Be(expected: reportDate);

        report.Value
            .Should()
            .Contain(expected: "User One");

        report.Value
            .Should()
            .Contain(expected: "/home");

        report.Value
            .Should()
            .Contain(expected: "/api/users");

        verificationContext.UserEvents
            .IgnoreQueryFilters()
            .Should()
            .HaveCount(expected: 3);

    }

    private static UserEvent CreateUserEvent(
        Tenant tenant,
        SSOUser user,
        DateTime createdOn,
        string eventName,
        string eventValue)
    {
        string sessionId = Guid.NewGuid()
            .ToString();

        Session session = new()
        {
            Id = sessionId,
            Value = "session-value",
            ExpiresAtTime = createdOn.AddHours(value: 1),
            AbsoluteExpiration = createdOn.AddHours(value: 1)
        };

        return new UserEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Tenant = tenant,
            CreatedBy = user.Id,
            CreatedByUser = user,
            CreatedOn = createdOn,
            EventName = eventName,
            Value = eventValue,
            SessionId = sessionId,
            Session = session
        };
    }
}