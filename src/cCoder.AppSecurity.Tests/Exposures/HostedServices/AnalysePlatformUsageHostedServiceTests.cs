// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.HostedServices;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Processings;
using Moq;

namespace cCoder.AppSecurity.Tests.Exposures.HostedServices;

public sealed partial class AnalysePlatformUsageHostedServiceTests
{
    private readonly Mock<IAnalysePlatformUsageProcessingService> analysePlatformUsageProcessingServiceMock = new();
    private readonly AppSecurityConfiguration appSecurityConfiguration = new();

    private AnalysePlatformUsageHostedService CreateService() =>
        new(analysePlatformUsageProcessingService: analysePlatformUsageProcessingServiceMock.Object, appSecurityConfiguration: appSecurityConfiguration);
}