// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.HostedServices;
using cCoder.AppSecurity.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;


namespace cCoder.AppSecurity.Tests;

public class HostedServicesRegistrationTests
{
    [Fact]
    public void AddAppSecurityHostedServices_RegistersHostedServices()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddAppSecurityHostedServices();

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(ITokenCleanerHostedService)
                && descriptor.ImplementationType == typeof(TokenCleanerHostedService));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IAnalysePlatformUsageHostedService)
                && descriptor.ImplementationType == typeof(AnalysePlatformUsageHostedService));
        Assert.Equal(
            2,
            services.Count(descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationFactory is not null));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(ITokenCleanerOrchestrationService)
                && descriptor.ImplementationType?.Name == "TokenCleanerOrchestrationService");
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IAnalysePlatformUsageOrchestrationService)
                && descriptor.ImplementationType?.Name == "AnalysePlatformUsageOrchestrationService");
    }
}