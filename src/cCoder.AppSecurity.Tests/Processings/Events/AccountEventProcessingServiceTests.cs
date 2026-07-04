using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings.Events;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;

namespace cCoder.AppSecurity.Tests.Processings.Events;

public partial class AccountEventProcessingServiceTests
{
    private readonly Mock<IAppService> appServiceMock;
    private readonly Mock<IUserService> userServiceMock;
    private readonly Mock<IRoleService> roleServiceMock;
    private readonly Mock<IUserRoleService> userRoleServiceMock;
    private readonly AccountEventProcessingService accountEventProcessingService;

    public AccountEventProcessingServiceTests()
    {
        appServiceMock = new Mock<IAppService>(MockBehavior.Strict);
        userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        roleServiceMock = new Mock<IRoleService>(MockBehavior.Strict);
        userRoleServiceMock = new Mock<IUserRoleService>(MockBehavior.Strict);

        accountEventProcessingService = new AccountEventProcessingService(
            appServiceMock.Object,
            userServiceMock.Object,
            roleServiceMock.Object,
            userRoleServiceMock.Object);
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
