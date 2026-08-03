// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Events;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Orchestrations.Events;

public partial class AccountEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCreateBootstrapUserBeforeFirstAppForProcessAsync()
    {
        // Given
        SecurityAccountEvent accountEvent = new()
        {
            RequestDomain = "https://localhost",
            User = new SSOUser
            {
                Id = "bootstrap.user",
                DisplayName = "Bootstrap User",
                Email = "bootstrap.user@example.com",
                LockoutEnabled = true
            }
        };

        appProcessingServiceMock
            .Setup(expression: service => service.GetByDomain(
                domain: "localhost"))
            .Returns(value: null);

        appProcessingServiceMock
            .Setup(expression: service => service.GetAll())
            .Returns(value: Array.Empty<cCoder.Data.Models.CMS.App>()
                .AsQueryable());

        userProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<User>()
                .AsQueryable());

        userProcessingServiceMock
            .Setup(expression: service => service.AddUserAsync(
                entity: It.Is<User>(match: user =>
                    user.Id == "Guest"
                    && user.DisplayName == "Guest"
                    && user.IsActive)))
            .ReturnsAsync(valueFunction: (User user) => user);

        userProcessingServiceMock
            .Setup(expression: service => service.AddUserAsync(
                entity: It.Is<User>(match: user =>
                    user.Id == accountEvent.User.Id
                    && user.DefaultCultureId == string.Empty
                    && user.DisplayName == accountEvent.User.DisplayName
                    && user.Email == accountEvent.User.Email
                    && user.IsActive)))
            .ReturnsAsync(valueFunction: (User user) => user);

        // When
        await accountEventOrchestrationService
            .ProcessSecurityAccountEventAsync(
                accountEvent: accountEvent);

        // Then
        userProcessingServiceMock.Verify(
            expression: service => service.AddUserAsync(
                entity: It.IsAny<User>()),
            times: Times.Exactly(callCount: 2));

        accountRoleAssignmentProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRetainBootstrapUserBeforeFirstAppForProcessAsync()
    {
        // Given
        User existingUser = new()
        {
            Id = "bootstrap.user",
            DisplayName = "Bootstrap User",
            Email = "bootstrap.user@example.com",
            IsActive = true
        };

        SecurityAccountEvent accountEvent = new()
        {
            RequestDomain = "https://localhost",
            User = new SSOUser
            {
                Id = existingUser.Id,
                DisplayName = existingUser.DisplayName,
                Email = existingUser.Email
            }
        };

        appProcessingServiceMock
            .Setup(expression: service => service.GetByDomain(
                domain: "localhost"))
            .Returns(value: null);

        appProcessingServiceMock
            .Setup(expression: service => service.GetAll())
            .Returns(value: Array.Empty<cCoder.Data.Models.CMS.App>()
                .AsQueryable());

        userProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[]
            {
                new User
                {
                    Id = "Guest"
                },
                existingUser
            }.AsQueryable());

        // When
        await accountEventOrchestrationService
            .ProcessSecurityAccountEventAsync(
                accountEvent: accountEvent);

        // Then
        userProcessingServiceMock.Verify(
            expression: service => service.GetAll(ignoreFilters: true),
            times: Times.Exactly(callCount: 2));

        userProcessingServiceMock.VerifyNoOtherCalls();
        accountRoleAssignmentProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldCreateAppUserAndAttachUsersRoleForProcessAsync()
    {
        // Given
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
            .Returns(value: Array.Empty<User>()
            .AsQueryable());

        userProcessingServiceMock
            .Setup(expression: service => service.AddUserAsync(entity: It.Is<User>(match: user =>
                user.Id == accountEvent.User.Id
                && user.DefaultCultureId == accountEvent.Culture
                && user.DisplayName == accountEvent.User.DisplayName
                && user.Email == accountEvent.User.Email
                && user.IsActive)))
            .ReturnsAsync(valueFunction: (User user) => user);

        accountRoleAssignmentProcessingServiceMock
            .Setup(expression: service => service.AttachUsersRoleAsync(
                user: It.Is<User>(
                    match: user => user.Id == accountEvent.User.Id),
                appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await accountEventOrchestrationService.ProcessSecurityAccountEventAsync(
            accountEvent: accountEvent);

        // Then
        userProcessingServiceMock.Verify(
expression: service => service.AddUserAsync(entity: It.IsAny<User>()),
times: Times.Once);

        accountRoleAssignmentProcessingServiceMock.Verify(
            expression: service => service.AttachUsersRoleAsync(
user: It.IsAny<User>(),
appId: app.Id),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldUpdateExistingUserFromTrustedAccountEventAsync()
    {
        // Given
        var app = CreateApp();

        User existingUser = new()
        {
            Id = "existing.user",
            DefaultCultureId = "en-GB",
            DisplayName = "Old name",
            Email = "old@example.com",
            IsActive = false
        };

        SecurityAccountEvent accountEvent = new()
        {
            RequestDomain = "https://example.com",
            Culture = "fr-FR",
            User = new SSOUser
            {
                Id = existingUser.Id,
                DisplayName = "Updated name",
                Email = "updated@example.com",
                LockoutEnabled = false
            }
        };

        appProcessingServiceMock
            .Setup(expression: service => service.GetByDomain(
                domain: app.Domain))
            .Returns(value: app);

        userProcessingServiceMock
            .Setup(expression: service => service.GetAll(
                ignoreFilters: true))
            .Returns(value: new[] { existingUser }.AsQueryable());

        userProcessingServiceMock
            .Setup(expression: service =>
                service.UpdateUserFromAccountEventAsync(
                    entity: It.Is<User>(match: user =>
                        user.Id == existingUser.Id
                        && user.DefaultCultureId == "fr-FR"
                        && user.DisplayName == "Updated name"
                        && user.Email == "updated@example.com"
                        && user.IsActive)))
            .ReturnsAsync(valueFunction: (User user) => user);

        accountRoleAssignmentProcessingServiceMock
            .Setup(expression: service => service.AttachUsersRoleAsync(
                user: It.Is<User>(match: user =>
                    user.Id == existingUser.Id),
                appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await accountEventOrchestrationService
            .ProcessSecurityAccountEventAsync(
                accountEvent: accountEvent);

        // Then
        userProcessingServiceMock.Verify(
            expression: service =>
                service.UpdateUserFromAccountEventAsync(
                    entity: It.IsAny<User>()),
            times: Times.Once);

        accountRoleAssignmentProcessingServiceMock.Verify(
            expression: service => service.AttachUsersRoleAsync(
                user: It.IsAny<User>(),
                appId: app.Id),
            times: Times.Once);
    }
}