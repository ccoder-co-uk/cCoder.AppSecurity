using cCoder.AppSecurity.Exposures.HostedServices;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Orchestrations;
using Moq;

namespace cCoder.AppSecurity.Tests.Exposures.HostedServices;

public sealed partial class AnalysePlatformUsageHostedServiceTests
{
    private readonly Mock<IAnalysePlatformUsageOrchestrationService> analysePlatformUsageOrchestrationServiceMock = new();
    private readonly AppSecurityConfiguration appSecurityConfiguration = new();

    private AnalysePlatformUsageHostedService CreateService() =>
        new(analysePlatformUsageOrchestrationServiceMock.Object, appSecurityConfiguration);
}
