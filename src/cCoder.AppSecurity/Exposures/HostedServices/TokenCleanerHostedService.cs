// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Models;
using Microsoft.Extensions.Hosting;


namespace cCoder.AppSecurity.Exposures.HostedServices;

public sealed class TokenCleanerHostedService(
    ITokenCleanerService tokenCleanerService,
    AppSecurityConfiguration appSecurityConfiguration)
    : BackgroundService, ITokenCleanerHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (appSecurityConfiguration.IsMigrating)
        {
            return;
        }

        await tokenCleanerService.RunAsync(cancellationToken: stoppingToken);

        using PeriodicTimer timer = new(period: TimeSpan.FromMinutes(minutes: 1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: stoppingToken))
        {
            await tokenCleanerService.RunAsync(cancellationToken: stoppingToken);
        }
    }
}