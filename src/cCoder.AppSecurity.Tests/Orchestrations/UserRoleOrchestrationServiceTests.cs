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

public partial class UserRoleOrchestrationServiceTests
{
    private readonly Mock<IUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly Mock<IUserRoleEventProcessingService> userRoleEventProcessingServiceMock;
    private readonly UserRoleOrchestrationService orchestrationService;

    public UserRoleOrchestrationServiceTests()
    {
        userRoleProcessingServiceMock = new Mock<IUserRoleProcessingService>(behavior: MockBehavior.Strict);
        userRoleEventProcessingServiceMock = new Mock<IUserRoleEventProcessingService>(behavior: MockBehavior.Strict);
        orchestrationService = new UserRoleOrchestrationService(
processingService:             userRoleProcessingServiceMock.Object,
eventService:             userRoleEventProcessingServiceMock.Object
        );
    }

    private static UserRole CreateRandomUserRole() => Builder<UserRole>.CreateNew().Build();
}