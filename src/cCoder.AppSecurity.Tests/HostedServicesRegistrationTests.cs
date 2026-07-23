// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.HostedServices;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;


namespace cCoder.AppSecurity.Tests;

public partial class HostedServicesRegistrationTests
{
    [Fact]
    public void AddAppSecurityHostedServices_RegistersHostedServices()
    {
        // Given
        IServiceCollection services = new ServiceCollection();

        // When
        services.AddAppSecurityHostedServices(
            configure: null);

        // Then
        Assert.Contains(
            collection: services,
            predicate: descriptor => descriptor.ServiceType == typeof(ITokenCleanerHostedService)
                && descriptor.ImplementationType == typeof(TokenCleanerHostedService));

        Assert.Contains(
            collection: services,
            predicate: descriptor => descriptor.ServiceType == typeof(IAnalysePlatformUsageHostedService)
                && descriptor.ImplementationType == typeof(AnalysePlatformUsageHostedService));

        Assert.Equal(
            expected: 2,
            actual: services.Count(predicate: descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationFactory is not null));

        Assert.Contains(
            collection: services,
            predicate: descriptor => descriptor.ServiceType == typeof(ITokenCleanerService)
                && descriptor.ImplementationType?.Name == "TokenCleanerService");

        Assert.Contains(
            collection: services,
            predicate: descriptor => descriptor.ServiceType == typeof(IAnalysePlatformUsageProcessingService)
                && descriptor.ImplementationType?.Name == "AnalysePlatformUsageProcessingService");
    }
}