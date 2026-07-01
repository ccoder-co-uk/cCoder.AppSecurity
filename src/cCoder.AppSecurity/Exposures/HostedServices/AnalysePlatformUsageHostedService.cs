using cCoder.AppSecurity.Services.Orchestrations;
using Microsoft.Extensions.Hosting;


namespace cCoder.AppSecurity.Exposures.HostedServices;

public sealed class AnalysePlatformUsageHostedService(
    IAnalysePlatformUsageOrchestrationService analysePlatformUsageOrchestrationService)
    : BackgroundService, IAnalysePlatformUsageHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        await analysePlatformUsageOrchestrationService.RunAsync(stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromDays(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            await analysePlatformUsageOrchestrationService.RunAsync(stoppingToken);
    }
}
