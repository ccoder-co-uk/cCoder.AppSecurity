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
        userBrokerMock
            .Setup(broker => broker.GetAllUsers(true))
            .Returns(Array.Empty<User>().AsQueryable());
        userBrokerMock
            .Setup(broker => broker.AddUserAsync(It.Is<User>(user =>
                user.Id == accountEvent.User.Id
                && user.DefaultCultureId == accountEvent.Culture
                && user.DisplayName == accountEvent.User.DisplayName
                && user.Email == accountEvent.User.Email
                && user.IsActive)))
            .ReturnsAsync((User user) => user);
        roleBrokerMock
            .Setup(broker => broker.GetAllRoles(true))
            .Returns(new[] { usersRole }.AsQueryable());
        userRoleBrokerMock
            .Setup(broker => broker.GetAllUserRoles(true))
            .Returns(Array.Empty<UserRole>().AsQueryable());
        userRoleBrokerMock
            .Setup(broker => broker.AddUserRoleAsync(It.Is<UserRole>(userRole =>
                userRole.UserId == accountEvent.User.Id
                && userRole.RoleId == usersRole.Id)))
            .ReturnsAsync((UserRole userRole) => userRole);

        await accountEventProcessingService.ProcessAsync(accountEvent);

        userBrokerMock.Verify(
            broker => broker.AddUserAsync(It.IsAny<User>()),
            Times.Once);
        userRoleBrokerMock.Verify(
            broker => broker.AddUserRoleAsync(It.IsAny<UserRole>()),
            Times.Once);
    }
}
