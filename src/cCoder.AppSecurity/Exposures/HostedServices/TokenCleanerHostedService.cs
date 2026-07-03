using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Models;
using Microsoft.Extensions.Hosting;


namespace cCoder.AppSecurity.Exposures.HostedServices;

public sealed class TokenCleanerHostedService(
    ITokenCleanerOrchestrationService tokenCleanerOrchestrationService,
    AppSecurityConfiguration appSecurityConfiguration)
    : BackgroundService, ITokenCleanerHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (appSecurityConfiguration.IsMigrating)
            return;

        await tokenCleanerOrchestrationService.RunAsync(stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            await tokenCleanerOrchestrationService.RunAsync(stoppingToken);
    }
}
