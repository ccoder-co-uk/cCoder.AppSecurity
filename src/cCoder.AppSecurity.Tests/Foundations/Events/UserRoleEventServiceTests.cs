// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using Moq;
using IUserRoleEventBroker = cCoder.AppSecurity.Brokers.Events.IUserRoleEventBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class UserRoleEventServiceTests
{
    private readonly Mock<IUserRoleEventBroker> userRoleEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.AppSecurity.Services.Foundations.Events.UserRoleEventService service;
    private const string CurrentUserId = "test-user";

    public UserRoleEventServiceTests()
    {
        userRoleEventBrokerMock = new Mock<IUserRoleEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        userRoleEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.AppSecurity.Services.Foundations.Events.UserRoleEventService(
            userRoleEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}