// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class PrivilegeOrchestrationServiceTests
{
    private readonly Mock<IPrivilegeProcessingService> privilegeProcessingServiceMock;
    private readonly Mock<IPrivilegeEventProcessingService> privilegeEventProcessingServiceMock;
    private readonly PrivilegeOrchestrationService orchestrationService;

    public PrivilegeOrchestrationServiceTests()
    {
        privilegeProcessingServiceMock = new Mock<IPrivilegeProcessingService>(behavior: MockBehavior.Strict);
        privilegeEventProcessingServiceMock = new Mock<IPrivilegeEventProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new PrivilegeOrchestrationService(
processingService: privilegeProcessingServiceMock.Object,
eventService: privilegeEventProcessingServiceMock.Object
        );
    }

    private static Privilege CreateRandomPrivilege() =>
        Builder<Privilege>.CreateNew()
        .Build();
}