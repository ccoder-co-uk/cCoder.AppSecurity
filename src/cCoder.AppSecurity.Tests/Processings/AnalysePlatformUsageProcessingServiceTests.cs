// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Security.Data.EF;
using cCoder.Security.Models.Configurations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class AnalysePlatformUsageProcessingServiceTests
{
    private readonly SqliteConnection connection;
    private readonly DbContextOptions<SecurityDbContext> options;
    private readonly SecurityDbContext securityDbContext;
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

#pragma warning disable STXFORMAT005
        analysePlatformUsageServiceMock = new Mock<IAnalysePlatformUsageService>(
            behavior: MockBehavior.Loose);
        analysePlatformUsageServiceMock.SetReturnsDefault(value: securityDbContext);
#pragma warning restore STXFORMAT005

        processingService = new AnalysePlatformUsageProcessingService(
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