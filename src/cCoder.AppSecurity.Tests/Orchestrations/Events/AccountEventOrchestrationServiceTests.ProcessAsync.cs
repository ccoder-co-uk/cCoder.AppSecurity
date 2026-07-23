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
            .Setup(expression: service => service.GetByDomain(domain: app.Domain))
            .Returns(value: app);
        userProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<User>().AsQueryable());
        userProcessingServiceMock
            .Setup(expression: service => service.AddUserAsync(entity: It.Is<User>(user =>
                user.Id == accountEvent.User.Id
                && user.DefaultCultureId == accountEvent.Culture
                && user.DisplayName == accountEvent.User.DisplayName
                && user.Email == accountEvent.User.Email
                && user.IsActive)))
            .ReturnsAsync(valueFunction: (User user) => user);
        accountRoleAssignmentProcessingServiceMock
            .Setup(expression: service => service.AttachUsersRoleAsync(
user:                 It.Is<User>(user => user.Id == accountEvent.User.Id),
appId:                 app.Id))
            .Returns(value: ValueTask.CompletedTask);

        await accountEventOrchestrationService.ProcessSecurityAccountEventAsync(accountEvent: accountEvent);

        userProcessingServiceMock.Verify(
expression:             service => service.AddUserAsync(entity: It.IsAny<User>()),
times:             Times.Once);
        accountRoleAssignmentProcessingServiceMock.Verify(
            expression: service => service.AttachUsersRoleAsync(
user:                 It.IsAny<User>(),
appId:                 app.Id),
            times: Times.Once);
    }
}