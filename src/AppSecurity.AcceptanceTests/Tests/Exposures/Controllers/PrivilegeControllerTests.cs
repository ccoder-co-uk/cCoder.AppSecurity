// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Exposures.Controllers;
using Moq;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public partial class PrivilegeControllerTests
{
    private readonly Mock<IPrivilegeManager> privilegeManagerMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    private PrivilegeController CreateController() =>
        new(service: privilegeManagerMock.Object, loggingBroker: loggingBrokerMock.Object);
}