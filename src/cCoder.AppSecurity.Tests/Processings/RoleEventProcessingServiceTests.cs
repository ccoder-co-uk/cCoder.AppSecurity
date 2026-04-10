using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.AppSecurity.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleEventProcessingServiceTests
{
    private readonly Mock<IRoleEventService> roleEventServiceMock;
    private readonly RoleEventProcessingService service;

    public RoleEventProcessingServiceTests()
    {
        roleEventServiceMock = new Mock<IRoleEventService>(MockBehavior.Strict);
        service = new RoleEventProcessingService(roleEventServiceMock.Object);
    }

    private static Role CreateRandomRole() =>
        Builder<Role>.CreateNew().Build();
}











