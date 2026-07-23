// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using Moq;
using IPrivilegeEventBroker = cCoder.AppSecurity.Brokers.Events.IPrivilegeEventBroker;


namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class PrivilegeEventServiceTests
{
    private readonly Mock<IPrivilegeEventBroker> privilegeEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoMock;
    private readonly cCoder.AppSecurity.Services.Foundations.Events.PrivilegeEventService service;
    private const string CurrentUserId = "test-user";

    public PrivilegeEventServiceTests()
    {
        privilegeEventBrokerMock = new Mock<IPrivilegeEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        privilegeEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();

        authInfoMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.AppSecurity.Services.Foundations.Events.PrivilegeEventService(
privilegeEventBroker: privilegeEventBrokerMock.Object,
authInfoBroker: authInfoMock.Object
        );
    }
}