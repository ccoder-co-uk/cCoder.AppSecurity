// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies.HostedServices;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Foundations;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.AppSecurity.Tests.Exposures.HostedServices;

public sealed partial class TokenCleanerHostedServiceTests
{
    private readonly Mock<ITokenCleanerService> tokenCleanerServiceMock = new();
    private readonly AppSecurityConfiguration appSecurityConfiguration = new();

    private TokenCleanerHostedService CreateService()
    {
        ServiceCollection services = new();

        services.AddSingleton(
            implementationInstance: tokenCleanerServiceMock.Object);

        ServiceProvider provider = services.BuildServiceProvider();

        return new TokenCleanerHostedService(
            serviceScopeFactory:
                provider.GetRequiredService<IServiceScopeFactory>(),
            appSecurityConfiguration: appSecurityConfiguration);
    }
}