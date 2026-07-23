// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Processings;
using cCoder.AppSecurity.Models;
using Microsoft.Extensions.Hosting;


namespace cCoder.AppSecurity.Exposures.HostedServices;

public sealed class AnalysePlatformUsageHostedService(
    IAnalysePlatformUsageProcessingService analysePlatformUsageProcessingService,
    AppSecurityConfiguration appSecurityConfiguration)
    : BackgroundService, IAnalysePlatformUsageHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (appSecurityConfiguration.IsMigrating)
        {
            return;
        }

        await analysePlatformUsageProcessingService.RunAsync(cancellationToken: stoppingToken);

        using PeriodicTimer timer = new(period: TimeSpan.FromDays(days: 1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: stoppingToken))
        {
            await analysePlatformUsageProcessingService.RunAsync(cancellationToken: stoppingToken);
        }
    }
}