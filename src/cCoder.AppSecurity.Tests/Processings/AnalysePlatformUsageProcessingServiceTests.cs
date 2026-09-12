// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.AppSecurity.Brokers.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Models.Configurations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text.Json;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class AnalysePlatformUsageProcessingServiceTests
{
    private readonly SqliteConnection connection;
    private readonly DbContextOptions<SecurityDbContext> options;
    private readonly SecurityDbContext securityDbContext;
    private readonly Mock<ISecurityDbContextBroker> securityDbContextBrokerMock;
    private readonly Mock<IAnalysePlatformUsageService> analysePlatformUsageServiceMock;
    private readonly AnalysePlatformUsageProcessingService processingService;

    public AnalysePlatformUsageProcessingServiceTests()
    {
        connection = new SqliteConnection(connectionString: "Data Source=:memory:");
        connection.Open();

        options = new DbContextOptionsBuilder<SecurityDbContext>()
            .UseSqlite(connection: connection)
            .Options;

        securityDbContext = CreateSecurityDbContext();

        securityDbContext.Database.EnsureCreated();

        securityDbContextBrokerMock = new Mock<ISecurityDbContextBroker>(
            behavior: MockBehavior.Loose);

        securityDbContextBrokerMock
            .Setup(expression: broker => broker.SelectUserEventDates())
            .Returns(valueFunction: () => securityDbContext.UserEvents
                .IgnoreQueryFilters()
                .Select(selector: userEvent => userEvent.CreatedOn)
                .AsEnumerable()
                .Select(selector: createdOn => createdOn.Date)
                .Distinct()
                .OrderByDescending(keySelector: date => date)
                .ToArray());

        securityDbContextBrokerMock
            .Setup(expression: broker => broker.SelectTenantIds())
            .Returns(valueFunction: () => securityDbContext.Tenants
                .IgnoreQueryFilters()
                .Select(selector: tenant => tenant.Id)
                .ToArray());

        securityDbContextBrokerMock
            .Setup(expression: broker => broker.SelectTenantAnalysis(
                tenantId: It.IsAny<string>(),
                createdOn: It.IsAny<DateTime>()))
            .Returns(valueFunction: (string tenantId, DateTime createdOn) =>
                securityDbContext.TenantAnalysis
                    .IgnoreQueryFilters()
                    .FirstOrDefault(predicate: analysis =>
                        analysis.TenantId == tenantId
                        && analysis.CreatedOn == createdOn
                        && analysis.Name == "User Activity (Daily)"));

        securityDbContextBrokerMock
            .Setup(expression: broker => broker.SelectUserActivities(
                tenantId: It.IsAny<string>(),
                from: It.IsAny<DateTime>(),
                to: It.IsAny<DateTime>()))
            .Returns(valueFunction: (string tenantId, DateTime from, DateTime to) =>
                securityDbContext.UserEvents
                    .IgnoreQueryFilters()
                    .Where(predicate: userEvent =>
                        userEvent.CreatedOn >= from
                        && userEvent.CreatedOn <= to
                        && userEvent.TenantId == tenantId)
                    .Select(selector: userEvent => new cCoder.Security.Models.Entities.UserActivity
                    {
                        TenantId = userEvent.TenantId,
                        EventName = userEvent.EventName,
                        EventValue = userEvent.Value,
                        EventCreatedOn = userEvent.CreatedOn,
                        SessionId = userEvent.SessionId,
                        UserId = userEvent.CreatedBy,
                        UserDisplayName = userEvent.CreatedByUser.DisplayName,
                        UserEmail = userEvent.CreatedByUser.Email
                    })
                    .ToArray());

        securityDbContextBrokerMock
            .Setup(expression: broker => broker.AddTenantAnalysesAsync(
                newTenantAnalyses: It.IsAny<cCoder.Security.Models.Entities.TenantAnalysis[]>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .Returns(valueFunction: async (
                cCoder.Security.Models.Entities.TenantAnalysis[] tenantAnalyses,
                CancellationToken cancellationToken) =>
            {
                securityDbContext.AddRange(entities: tenantAnalyses);
                await securityDbContext.SaveChangesAsync(
                    cancellationToken: cancellationToken);
            });

        securityDbContextBrokerMock
            .Setup(expression: broker => broker.DeleteUserEventsBeforeAsync(
                createdBefore: It.IsAny<DateTime>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .Returns(valueFunction: async (
                DateTime createdBefore,
                CancellationToken cancellationToken) =>
            {
                cCoder.Security.Models.Entities.UserEvent[] oldEvents =
                    securityDbContext.UserEvents
                        .IgnoreQueryFilters()
                        .Where(predicate: userEvent => userEvent.CreatedOn < createdBefore)
                        .ToArray();

                securityDbContext.RemoveRange(entities: oldEvents);
                await securityDbContext.SaveChangesAsync(
                    cancellationToken: cancellationToken);
            });

#pragma warning disable STXFORMAT005
        analysePlatformUsageServiceMock = new Mock<IAnalysePlatformUsageService>(
            behavior: MockBehavior.Loose);
        analysePlatformUsageServiceMock.SetReturnsDefault(value: securityDbContext);
#pragma warning restore STXFORMAT005

        analysePlatformUsageServiceMock
            .Setup(expression: service => service.Serialize(value: It.IsAny<object>()))
            .Returns(valueFunction: (object value) => JsonSerializer.Serialize(value: value));

        processingService = new AnalysePlatformUsageProcessingService(
            securityDbContextBroker: securityDbContextBrokerMock.Object,
            analysePlatformUsageService: analysePlatformUsageServiceMock.Object);
    }

    private SecurityDbContext CreateSecurityDbContext() =>
        new TestSecurityDbContext(
            authInfo: Mock.Of<ISSOAuthInfo>(),
            options: options);

    private sealed class TestSecurityDbContext(
        ISSOAuthInfo authInfo,
        DbContextOptions<SecurityDbContext> options)
        : SecurityDbContext(authInfo: authInfo, options: options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder: modelBuilder);

            modelBuilder.Entity<cCoder.Security.Models.Entities.UserEvent>()
                .Property(propertyExpression: userEvent => userEvent.CreatedOn)
                .HasConversion<long>();
        }
    }
}