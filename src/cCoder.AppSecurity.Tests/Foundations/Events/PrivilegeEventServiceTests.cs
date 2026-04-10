using cCoder.Data;
using Moq;
using IPrivilegeEventBroker = cCoder.AppSecurity.Brokers.Events.IPrivilegeEventBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class PrivilegeEventServiceTests
{
    private readonly Mock<IPrivilegeEventBroker> privilegeEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.AppSecurity.Services.Foundations.Events.PrivilegeEventService service;
    private const string CurrentUserId = "test-user";

    public PrivilegeEventServiceTests()
    {
        privilegeEventBrokerMock = new Mock<IPrivilegeEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        privilegeEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.AppSecurity.Services.Foundations.Events.PrivilegeEventService(
            privilegeEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}










