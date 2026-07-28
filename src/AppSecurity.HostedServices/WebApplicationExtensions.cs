// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.AppSecurity.Exposures.EventHandlers;

namespace AppSecurity.HostedServices;

public static class WebApplicationExtensions
{
    public static void UseHostedServicesApplication(
        this WebApplication app)
    {
        app.MapGet(
            pattern: "/",
            handler: (IHostEnvironment environment) =>
                Results.Text(
                    content: BuildHostedServicesReport(
                        environment: environment),
                    contentType: "text/plain"));

        app.MapGet(
            pattern: "/Health",
            handler: () => Results.Text(content: "Healthy"));

        using IServiceScope serviceScope = app.Services.CreateScope();
        IServiceProvider services = serviceScope.ServiceProvider;

        foreach (IAppSecurityEventHandlers handlers
            in services.GetServices<IAppSecurityEventHandlers>())
        {
            handlers.ListenToAppCreateAndUpdateEvents();
            handlers.ListenToSecurityAccountEvents();
            handlers.ListenToAppDeleteEvents();
        }
    }

    private static string BuildHostedServicesReport(
        IHostEnvironment environment) =>
        string.Join(
            separator: Environment.NewLine,
            values:
            [
                "cCoder.AppSecurity Hosted Services",
                "Status: Healthy",
                $"Environment: {environment.EnvironmentName}",
                "Health: /Health",
                string.Empty,
                "Hosted background services:",
                "- TokenCleanerHostedService -> ITokenCleanerService.RunAsync every 1 minute",
                "- AnalysePlatformUsageHostedService -> IAnalysePlatformUsageProcessingService.RunAsync every 1 day",
                string.Empty,
                "Hosted event listeners:",
                "- app_add/app_update -> app security setup and update handlers",
                "- app_delete -> app security cleanup handlers",
                "- security account lifecycle events -> app-local user setup handlers",
            ]);
}