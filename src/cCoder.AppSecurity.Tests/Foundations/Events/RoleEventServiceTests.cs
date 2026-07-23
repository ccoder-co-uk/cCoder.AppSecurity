// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using Moq;
using IRoleEventBroker = cCoder.AppSecurity.Brokers.Events.IRoleEventBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class RoleEventServiceTests
{
    private readonly Mock<IRoleEventBroker> roleEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoMock;
    private readonly cCoder.AppSecurity.Services.Foundations.Events.RoleEventService service;
    private const string CurrentUserId = "test-user";

    public RoleEventServiceTests()
    {
        roleEventBrokerMock = new Mock<IRoleEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        roleEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();

        authInfoMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.AppSecurity.Services.Foundations.Events.RoleEventService(
roleEventBroker: roleEventBrokerMock.Object,
authInfoBroker: authInfoMock.Object
        );
    }
}