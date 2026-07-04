using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Storages;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings.Events;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;

namespace cCoder.AppSecurity.Tests.Processings.Events;

public partial class AccountEventProcessingServiceTests
{
    private readonly Mock<IAppService> appServiceMock;
    private readonly Mock<IUserBroker> userBrokerMock;
    private readonly Mock<IRoleBroker> roleBrokerMock;
    private readonly Mock<IUserRoleBroker> userRoleBrokerMock;
    private readonly AccountEventProcessingService accountEventProcessingService;

    public AccountEventProcessingServiceTests()
    {
        appServiceMock = new Mock<IAppService>(MockBehavior.Strict);
        userBrokerMock = new Mock<IUserBroker>(MockBehavior.Strict);
        roleBrokerMock = new Mock<IRoleBroker>(MockBehavior.Strict);
        userRoleBrokerMock = new Mock<IUserRoleBroker>(MockBehavior.Strict);

        accountEventProcessingService = new AccountEventProcessingService(
            appServiceMock.Object,
            userBrokerMock.Object,
            roleBrokerMock.Object,
            userRoleBrokerMock.Object);
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
