// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Orchestrations.Events;

public partial class AccountEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCreateAppUserAndAttachUsersRoleForProcessAsync()
    {
        var app = CreateApp();
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

        appProcessingServiceMock
            .Setup(service => service.GetByDomain(app.Domain))
            .Returns(app);
        userProcessingServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(Array.Empty<User>().AsQueryable());
        userProcessingServiceMock
            .Setup(service => service.AddUserAsync(It.Is<User>(user =>
                user.Id == accountEvent.User.Id
                && user.DefaultCultureId == accountEvent.Culture
                && user.DisplayName == accountEvent.User.DisplayName
                && user.Email == accountEvent.User.Email
                && user.IsActive)))
            .ReturnsAsync((User user) => user);
        accountRoleAssignmentProcessingServiceMock
            .Setup(service => service.AttachUsersRoleAsync(
                It.Is<User>(user => user.Id == accountEvent.User.Id),
                app.Id))
            .Returns(ValueTask.CompletedTask);

        await accountEventOrchestrationService.ProcessSecurityAccountEventAsync(accountEvent);

        userProcessingServiceMock.Verify(
            service => service.AddUserAsync(It.IsAny<User>()),
            Times.Once);
        accountRoleAssignmentProcessingServiceMock.Verify(
            expression: service => service.AttachUsersRoleAsync(
                It.IsAny<User>(),
                app.Id),
            times: Times.Once);
    }
}