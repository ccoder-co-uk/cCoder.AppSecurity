// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.HostedServices;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Orchestrations;
using Moq;

namespace cCoder.AppSecurity.Tests.Exposures.HostedServices;

public sealed partial class TokenCleanerHostedServiceTests
{
    private readonly Mock<ITokenCleanerOrchestrationService> tokenCleanerOrchestrationServiceMock = new();
    private readonly AppSecurityConfiguration appSecurityConfiguration = new();

    private TokenCleanerHostedService CreateService() =>
        new(tokenCleanerOrchestrationServiceMock.Object, appSecurityConfiguration);
}