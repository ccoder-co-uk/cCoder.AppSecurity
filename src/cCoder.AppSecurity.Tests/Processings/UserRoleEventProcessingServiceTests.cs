using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.AppSecurity.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleEventProcessingServiceTests
{
    private readonly Mock<IUserRoleEventService> userRoleEventServiceMock;
    private readonly UserRoleEventProcessingService service;

    public UserRoleEventProcessingServiceTests()
    {
        userRoleEventServiceMock = new Mock<IUserRoleEventService>(MockBehavior.Strict);
        service = new UserRoleEventProcessingService(userRoleEventServiceMock.Object);
    }

    private static UserRole CreateRandomUserRole() =>
        Builder<UserRole>.CreateNew().Build();
}











