// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using Moq;
using IUserEventBroker = cCoder.AppSecurity.Brokers.Events.IUserEventBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class UserEventServiceTests
{
    private readonly Mock<IUserEventBroker> userEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.AppSecurity.Services.Foundations.Events.UserEventService service;
    private const string CurrentUserId = "test-user";

    public UserEventServiceTests()
    {
        userEventBrokerMock = new Mock<IUserEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        userEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.AppSecurity.Services.Foundations.Events.UserEventService(
            userEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}