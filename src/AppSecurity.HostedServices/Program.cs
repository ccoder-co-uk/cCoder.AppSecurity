using cCoder.AppSecurity;
using cCoder.Data;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Objects;

namespace AppSecurity.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string coreConnection = builder.Configuration.GetConnectionString("Core")
            ?? throw new InvalidOperationException("ConnectionStrings:Core is required.");

        string ssoConnection = builder.Configuration.GetConnectionString("SSO")
            ?? throw new InvalidOperationException("ConnectionStrings:SSO is required.");

        Config config = new();
        builder.Configuration.Bind(config);
        builder.Services.AddSingleton(config);

        builder.Services.AddSecurity((services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services, ssoConnection);
            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                builder.Configuration.GetSection("Settings")["DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(builder.Services, coreConnection);
        builder.Services.AddAppSecurityHostedServices(appSecurityConfiguration =>
        {
            appSecurityConfiguration.IsMigrating =
                builder.Configuration.GetValue<int?>("MIGRATING") == 1
                || builder.Configuration.GetValue<bool?>("AppSecurity:IsMigrating") == true;
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        WebApplication app = builder.Build();
        app.MapGet("/", (IHostEnvironment environment) =>
            Results.Text(BuildHostedServicesReport(environment), "text/plain"));
        app.MapGet("/Health", () => Results.Text("Healthy"));
        app.StartAppSecurityHostedServices();
        app.Run();
    }

    private static string BuildHostedServicesReport(IHostEnvironment environment) =>
        string.Join(
            Environment.NewLine,
            "cCoder.AppSecurity Hosted Services",
            "Status: Healthy",
            $"Environment: {environment.EnvironmentName}",
            "Health: /Health",
            string.Empty,
            "Hosted background services:",
            "- TokenCleanerHostedService -> ITokenCleanerOrchestrationService.RunAsync every 1 minute",
            "- AnalysePlatformUsageHostedService -> IAnalysePlatformUsageOrchestrationService.RunAsync every 1 day",
            string.Empty,
            "Hosted event listeners:",
            "- app_add/app_update -> app security setup and update handlers",
            "- app_delete -> app security cleanup handlers",
            "- security account lifecycle events -> app-local user setup handlers");
}
