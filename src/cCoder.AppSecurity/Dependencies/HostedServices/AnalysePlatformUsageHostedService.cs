// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Processings;
using cCoder.AppSecurity.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace cCoder.AppSecurity.Dependencies.HostedServices;

public sealed class AnalysePlatformUsageHostedService(
    IServiceScopeFactory serviceScopeFactory,
    AppSecurityConfiguration appSecurityConfiguration)
    : BackgroundService, IAnalysePlatformUsageHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (appSecurityConfiguration.IsMigrating)
        {
            return;
        }

        using IServiceScope scope =
            serviceScopeFactory.CreateScope();

        IAnalysePlatformUsageProcessingService
            analysePlatformUsageProcessingService =
                scope.ServiceProvider.GetRequiredService<
                    IAnalysePlatformUsageProcessingService>();

        await analysePlatformUsageProcessingService.RunAsync(cancellationToken: stoppingToken);

        using PeriodicTimer timer = new(period: TimeSpan.FromDays(days: 1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: stoppingToken))
        {
            await analysePlatformUsageProcessingService.RunAsync(cancellationToken: stoppingToken);
        }
    }
}