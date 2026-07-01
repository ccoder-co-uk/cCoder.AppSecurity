using cCoder.AppSecurity.Services.Orchestrations;
using Microsoft.Extensions.Hosting;


namespace cCoder.AppSecurity.Exposures.HostedServices;

public sealed class TokenCleanerHostedService(
    ITokenCleanerOrchestrationService tokenCleanerOrchestrationService)
    : BackgroundService, ITokenCleanerHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        await tokenCleanerOrchestrationService.RunAsync(stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            await tokenCleanerOrchestrationService.RunAsync(stoppingToken);
    }
}
