// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;

namespace cCoder.AppSecurity.Tests.Orchestrations.Events;

public partial class AccountEventOrchestrationServiceTests
{
    private readonly Mock<IAppProcessingService> appProcessingServiceMock;
    private readonly Mock<IUserProcessingService> userProcessingServiceMock;
    private readonly Mock<IAccountRoleAssignmentProcessingService> accountRoleAssignmentProcessingServiceMock;
    private readonly AccountEventOrchestrationService accountEventOrchestrationService;

    public AccountEventOrchestrationServiceTests()
    {
        appProcessingServiceMock = new Mock<IAppProcessingService>(behavior: MockBehavior.Strict);
        userProcessingServiceMock = new Mock<IUserProcessingService>(behavior: MockBehavior.Strict);

        accountRoleAssignmentProcessingServiceMock =
            new Mock<IAccountRoleAssignmentProcessingService>(
                behavior: MockBehavior.Strict);

        accountEventOrchestrationService = new AccountEventOrchestrationService(
            appProcessingService: appProcessingServiceMock.Object,
            userProcessingService: userProcessingServiceMock.Object,
            accountRoleAssignmentProcessingService: accountRoleAssignmentProcessingServiceMock.Object);
    }

    private static App CreateApp() =>
        new()
    {
        Id = 123,
        Domain = "example.com",
        DefaultCultureId = "en-GB"
    };

    private static Role CreateUsersRole(int appId) =>
        new()
    {
        Id = Guid.NewGuid(),
        AppId = appId,
        Name = "Users"
    };
}