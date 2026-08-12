// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations.Events;

public sealed partial class RoleEventServiceMappingTests
{
    [Fact]
    public async Task ShouldMapCompleteRoleWhenRaisingAddEventAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        Role role = new()
        {
            Id = roleId,
            AppId = 7,
            Name = "Users",
            App = new App { Id = 7, Name = "App", Domain = "app.example.com" },
            Users =
            [
                new UserRole
                {
                    RoleId = roleId,
                    UserId = "user-one",
                    User = new User
                    {
                        Id = "user-one",
                        DisplayName = "User One",
                        Email = "user@example.com"
                    }
                }
            ],
            Pages = [new PageRole { PageId = 11, RoleId = roleId }],
            Folders = [new FolderRole { FolderId = Guid.NewGuid(), RoleId = roleId }]
        };

        EventMessage<Role> capturedMessage = null;
        Mock<IRoleEventBroker> eventBrokerMock = new();
        Mock<IAuthInfoBroker> authInfoBrokerMock = new();

        authInfoBrokerMock
            .Setup(expression: broker => broker.GetSSOUserId())
            .Returns(value: "user-one");

        eventBrokerMock
            .Setup(expression: broker => broker.RaiseRoleAddEventAsync(
                message: It.IsAny<EventMessage<Role>>()))
            .Callback<EventMessage<Role>>(action: message => capturedMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        var service = new RoleEventService(
            roleEventBroker: eventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object);

        // When
        await service.RaiseRoleAddEventAsync(entity: role);

        // Then
        capturedMessage.Data.Users
            .Should()
            .ContainSingle();

        capturedMessage.Data.Pages
            .Should()
            .ContainSingle();

        capturedMessage.Data.Folders
            .Should()
            .ContainSingle();
    }
}