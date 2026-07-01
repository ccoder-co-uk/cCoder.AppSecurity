using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings.Events;

public partial class AccountEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldCreateAppUserAndAttachUsersRoleForProcessAsync()
    {
        App app = CreateApp();
        Role usersRole = CreateUsersRole(app.Id);
        SecurityAccountEvent accountEvent = new()
        {
            RequestDomain = "https://example.com",
            Culture = "fr-FR",
            User = new SSOUser
            {
                Id = "new.user",
                DisplayName = "New User",
                Email = "new.user@example.com"
            }
        };

        appServiceMock
            .Setup(service => service.GetByDomain(app.Domain))
            .Returns(app);
        userOrchestrationServiceMock
            .Setup(service => service.Get(accountEvent.User.Id))
            .Returns((User)null);
        userOrchestrationServiceMock
            .Setup(service => service.AddAsync(It.Is<User>(user =>
                user.Id == accountEvent.User.Id
                && user.DefaultCultureId == accountEvent.Culture
                && user.DisplayName == accountEvent.User.DisplayName
                && user.Email == accountEvent.User.Email
                && user.IsActive)))
            .ReturnsAsync((User user) => user);
        roleOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(new[] { usersRole }.AsQueryable());
        userRoleOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(Array.Empty<UserRole>().AsQueryable());
        userRoleOrchestrationServiceMock
            .Setup(service => service.SaveAsync(It.Is<UserRole>(userRole =>
                userRole.UserId == accountEvent.User.Id
                && userRole.RoleId == usersRole.Id)))
            .ReturnsAsync((UserRole userRole) => userRole);

        await accountEventProcessingService.ProcessAsync(accountEvent);

        userOrchestrationServiceMock.Verify(
            service => service.AddAsync(It.IsAny<User>()),
            Times.Once);
        userRoleOrchestrationServiceMock.Verify(
            service => service.SaveAsync(It.IsAny<UserRole>()),
            Times.Once);
    }
}
