// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using Moq;
using IRoleEventBroker = cCoder.AppSecurity.Brokers.Events.IRoleEventBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class RoleEventServiceTests
{
    private readonly Mock<IRoleEventBroker> roleEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.AppSecurity.Services.Foundations.Events.RoleEventService service;
    private const string CurrentUserId = "test-user";

    public RoleEventServiceTests()
    {
        roleEventBrokerMock = new Mock<IRoleEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        roleEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.AppSecurity.Services.Foundations.Events.RoleEventService(
            roleEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}