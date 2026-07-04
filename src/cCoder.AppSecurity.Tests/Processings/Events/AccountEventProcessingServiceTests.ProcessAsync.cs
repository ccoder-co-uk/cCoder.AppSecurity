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
        userServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(Array.Empty<User>().AsQueryable());
        userServiceMock
            .Setup(service => service.AddValidatedAsync(It.Is<User>(user =>
                user.Id == accountEvent.User.Id
                && user.DefaultCultureId == accountEvent.Culture
                && user.DisplayName == accountEvent.User.DisplayName
                && user.Email == accountEvent.User.Email
                && user.IsActive)))
            .ReturnsAsync((User user) => user);
        roleServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(new[] { usersRole }.AsQueryable());
        userRoleServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(Array.Empty<UserRole>().AsQueryable());
        userRoleServiceMock
            .Setup(service => service.AddValidatedAsync(It.Is<UserRole>(userRole =>
                userRole.UserId == accountEvent.User.Id
                && userRole.RoleId == usersRole.Id)))
            .ReturnsAsync((UserRole userRole) => userRole);

        await accountEventProcessingService.ProcessAsync(accountEvent);

        userServiceMock.Verify(
            service => service.AddValidatedAsync(It.IsAny<User>()),
            Times.Once);
        userRoleServiceMock.Verify(
            service => service.AddValidatedAsync(It.IsAny<UserRole>()),
            Times.Once);
    }
}
