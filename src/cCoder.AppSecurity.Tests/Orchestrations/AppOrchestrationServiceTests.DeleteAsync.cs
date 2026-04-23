using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteUserRolesBeforeRolesWhenDeleteAsync()
    {
        Mock<IAuthorizationBroker> authorizationBrokerMock = new(MockBehavior.Strict);
        Mock<IPrivilegeService> privilegeServiceMock = new(MockBehavior.Strict);
        Mock<IRoleOrchestrationService> roleOrchestrationServiceMock = new(MockBehavior.Strict);

        AppOrchestrationService orchestrationService = new(
            authorizationBrokerMock.Object,
            privilegeServiceMock.Object,
            roleOrchestrationServiceMock.Object);

        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 5,
            Name = "Administrators",
            Privs = "app_delete"
        };

        roleOrchestrationServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[] { role }.AsQueryable());

        roleOrchestrationServiceMock
            .Setup(x => x.DeleteValidatedAsync(role.Id))
            .Returns(ValueTask.CompletedTask);

        await orchestrationService.DeleteAsync(5);

        roleOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
        roleOrchestrationServiceMock.Verify(x => x.DeleteValidatedAsync(role.Id), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
        privilegeServiceMock.VerifyNoOtherCalls();
    }
}
