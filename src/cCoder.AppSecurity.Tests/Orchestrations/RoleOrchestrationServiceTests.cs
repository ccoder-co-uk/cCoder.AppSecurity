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

public partial class RoleOrchestrationServiceTests
{
    private readonly Mock<IRoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<IRoleEventProcessingService> roleEventProcessingServiceMock;
    private readonly RoleOrchestrationService orchestrationService;

    public RoleOrchestrationServiceTests()
    {
        roleProcessingServiceMock = new Mock<IRoleProcessingService>(behavior: MockBehavior.Strict);
        roleEventProcessingServiceMock = new Mock<IRoleEventProcessingService>(behavior: MockBehavior.Strict);
        orchestrationService = new RoleOrchestrationService(
processingService:             roleProcessingServiceMock.Object,
eventService:             roleEventProcessingServiceMock.Object
        );
    }

    private static Role CreateRandomRole() => Builder<Role>.CreateNew().Build();
}