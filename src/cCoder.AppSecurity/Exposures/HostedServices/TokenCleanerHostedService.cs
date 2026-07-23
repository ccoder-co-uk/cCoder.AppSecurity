// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        {
            return;
        }

        await tokenCleanerOrchestrationService.RunAsync(cancellationToken: stoppingToken);

        using PeriodicTimer timer = new(period: TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: stoppingToken))
        {
            await tokenCleanerOrchestrationService.RunAsync(cancellationToken: stoppingToken);
        }
    }
}