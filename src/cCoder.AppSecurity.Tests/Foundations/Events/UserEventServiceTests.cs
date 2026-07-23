// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using Moq;
using IUserEventBroker = cCoder.AppSecurity.Brokers.Events.IUserEventBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class UserEventServiceTests
{
    private readonly Mock<IUserEventBroker> userEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoMock;
    private readonly cCoder.AppSecurity.Services.Foundations.Events.UserEventService service;
    private const string CurrentUserId = "test-user";

    public UserEventServiceTests()
    {
        userEventBrokerMock = new Mock<IUserEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        userEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();

        authInfoMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.AppSecurity.Services.Foundations.Events.UserEventService(
userEventBroker: userEventBrokerMock.Object,
authInfoBroker: authInfoMock.Object
        );
    }
}