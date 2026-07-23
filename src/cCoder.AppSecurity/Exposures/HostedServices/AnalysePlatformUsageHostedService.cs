// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Models;
using Microsoft.Extensions.Hosting;


namespace cCoder.AppSecurity.Exposures.HostedServices;

public sealed class AnalysePlatformUsageHostedService(
    IAnalysePlatformUsageOrchestrationService analysePlatformUsageOrchestrationService,
    AppSecurityConfiguration appSecurityConfiguration)
    : BackgroundService, IAnalysePlatformUsageHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (appSecurityConfiguration.IsMigrating)
        {
            return;
        }

        await analysePlatformUsageOrchestrationService.RunAsync(cancellationToken: stoppingToken);

        using PeriodicTimer timer = new(period: TimeSpan.FromDays(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: stoppingToken))
        {
            await analysePlatformUsageOrchestrationService.RunAsync(cancellationToken: stoppingToken);
        }
    }
}