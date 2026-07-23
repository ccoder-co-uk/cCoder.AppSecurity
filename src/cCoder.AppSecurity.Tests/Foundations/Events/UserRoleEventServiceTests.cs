// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using Moq;
using IUserRoleEventBroker = cCoder.AppSecurity.Brokers.Events.IUserRoleEventBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class UserRoleEventServiceTests
{
    private readonly Mock<IUserRoleEventBroker> userRoleEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoMock;
    private readonly cCoder.AppSecurity.Services.Foundations.Events.UserRoleEventService service;
    private const string CurrentUserId = "test-user";

    public UserRoleEventServiceTests()
    {
        userRoleEventBrokerMock = new Mock<IUserRoleEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        userRoleEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: CurrentUserId);
        service = new cCoder.AppSecurity.Services.Foundations.Events.UserRoleEventService(
userRoleEventBroker:             userRoleEventBrokerMock.Object,
authInfoBroker:             authInfoMock.Object
        );
    }
}