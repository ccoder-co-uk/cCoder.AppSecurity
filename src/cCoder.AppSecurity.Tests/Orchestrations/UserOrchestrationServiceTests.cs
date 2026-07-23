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

public partial class UserOrchestrationServiceTests
{
    private readonly Mock<IUserProcessingService> userProcessingServiceMock;
    private readonly Mock<IUserEventProcessingService> userEventProcessingServiceMock;
    private readonly UserOrchestrationService orchestrationService;

    public UserOrchestrationServiceTests()
    {
        userProcessingServiceMock = new Mock<IUserProcessingService>(MockBehavior.Strict);
        userEventProcessingServiceMock = new Mock<IUserEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new UserOrchestrationService(
            userProcessingServiceMock.Object,
            userEventProcessingServiceMock.Object
        );
    }

    private static User CreateRandomUser() => Builder<User>.CreateNew().Build();
}