using cCoder.AppSecurity.Exposures.HostedServices;
using cCoder.AppSecurity.Services.Orchestrations;
using Moq;

namespace cCoder.AppSecurity.Tests.Exposures.HostedServices;

public sealed partial class TokenCleanerHostedServiceTests
{
    private readonly Mock<ITokenCleanerOrchestrationService> tokenCleanerOrchestrationServiceMock = new();

    private TokenCleanerHostedService CreateService() =>
        new(tokenCleanerOrchestrationServiceMock.Object);
}
