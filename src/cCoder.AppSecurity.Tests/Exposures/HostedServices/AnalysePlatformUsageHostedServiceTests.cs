// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies.HostedServices;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Processings;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.AppSecurity.Tests.Exposures.HostedServices;

public sealed partial class AnalysePlatformUsageHostedServiceTests
{
    private readonly Mock<IAnalysePlatformUsageProcessingService> analysePlatformUsageProcessingServiceMock = new();
    private readonly AppSecurityConfiguration appSecurityConfiguration = new();

    private AnalysePlatformUsageHostedService CreateService()
    {
        ServiceCollection services = new();

        services.AddSingleton(
            implementationInstance:
                analysePlatformUsageProcessingServiceMock.Object);

        ServiceProvider provider = services.BuildServiceProvider();

        return new AnalysePlatformUsageHostedService(
            serviceScopeFactory:
                provider.GetRequiredService<IServiceScopeFactory>(),
            appSecurityConfiguration: appSecurityConfiguration);
    }
}