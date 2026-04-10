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
        userRoleProcessingServiceMock = new Mock<IUserRoleProcessingService>(MockBehavior.Strict);
        userRoleEventProcessingServiceMock = new Mock<IUserRoleEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new UserRoleOrchestrationService(
            userRoleProcessingServiceMock.Object,
            userRoleEventProcessingServiceMock.Object
        );
    }

    private static UserRole CreateRandomUserRole() => Builder<UserRole>.CreateNew().Build();
}









