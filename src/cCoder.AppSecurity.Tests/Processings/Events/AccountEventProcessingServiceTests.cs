using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings.Events;
using Moq;

namespace cCoder.AppSecurity.Tests.Processings.Events;

public partial class AccountEventProcessingServiceTests
{
    private readonly Mock<IAccountEventOrchestrationService> accountEventOrchestrationServiceMock;
    private readonly AccountEventProcessingService accountEventProcessingService;

    public AccountEventProcessingServiceTests()
    {
        accountEventOrchestrationServiceMock = new Mock<IAccountEventOrchestrationService>(MockBehavior.Strict);

        accountEventProcessingService = new AccountEventProcessingService(
            accountEventOrchestrationServiceMock.Object);
    }
}
