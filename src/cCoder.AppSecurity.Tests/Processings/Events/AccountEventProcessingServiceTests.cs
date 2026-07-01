using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings.Events;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;

namespace cCoder.AppSecurity.Tests.Processings.Events;

public partial class AccountEventProcessingServiceTests
{
    private readonly Mock<IAppService> appServiceMock;
    private readonly Mock<IUserOrchestrationService> userOrchestrationServiceMock;
    private readonly Mock<IRoleOrchestrationService> roleOrchestrationServiceMock;
    private readonly Mock<IUserRoleOrchestrationService> userRoleOrchestrationServiceMock;
    private readonly AccountEventProcessingService accountEventProcessingService;

    public AccountEventProcessingServiceTests()
    {
        appServiceMock = new Mock<IAppService>(MockBehavior.Strict);
        userOrchestrationServiceMock = new Mock<IUserOrchestrationService>(MockBehavior.Strict);
        roleOrchestrationServiceMock = new Mock<IRoleOrchestrationService>(MockBehavior.Strict);
        userRoleOrchestrationServiceMock = new Mock<IUserRoleOrchestrationService>(MockBehavior.Strict);

        accountEventProcessingService = new AccountEventProcessingService(
            appServiceMock.Object,
            userOrchestrationServiceMock.Object,
            roleOrchestrationServiceMock.Object,
            userRoleOrchestrationServiceMock.Object);
    }

    private static App CreateApp() => new()
    {
        Id = 123,
        Domain = "example.com",
        DefaultCultureId = "en-GB"
    };

    private static Role CreateUsersRole(int appId) => new()
    {
        Id = Guid.NewGuid(),
        AppId = appId,
        Name = "Users"
    };
}
