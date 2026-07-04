using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;

namespace cCoder.AppSecurity.Tests.Orchestrations.Events;

public partial class AccountEventOrchestrationServiceTests
{
    private readonly Mock<IAppService> appServiceMock;
    private readonly Mock<IUserProcessingService> userProcessingServiceMock;
    private readonly Mock<IRoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<IUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly AccountEventOrchestrationService accountEventOrchestrationService;

    public AccountEventOrchestrationServiceTests()
    {
        appServiceMock = new Mock<IAppService>(MockBehavior.Strict);
        userProcessingServiceMock = new Mock<IUserProcessingService>(MockBehavior.Strict);
        roleProcessingServiceMock = new Mock<IRoleProcessingService>(MockBehavior.Strict);
        userRoleProcessingServiceMock = new Mock<IUserRoleProcessingService>(MockBehavior.Strict);

        accountEventOrchestrationService = new AccountEventOrchestrationService(
            appServiceMock.Object,
            userProcessingServiceMock.Object,
            roleProcessingServiceMock.Object,
            userRoleProcessingServiceMock.Object);
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
