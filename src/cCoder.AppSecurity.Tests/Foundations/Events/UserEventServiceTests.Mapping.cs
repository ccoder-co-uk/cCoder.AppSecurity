// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Foundations.Events;

public partial class UserEventServiceTests
{
#pragma warning disable STXFORMAT008
    [Fact]
    public async Task ShouldMapNestedUserDataWhenRaiseUserAddEventAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        Culture culture = new() { Id = "en-GB" };

        Role role = new()
        {
            Id = roleId,
            AppId = 7,
            Name = "Users",
            Description = "Application users",
            Privs = "page_read"
        };

        User entity = new()
        {
            Id = "user-one",
            DefaultCultureId = culture.Id,
            DefaultCulture = culture,
            Roles =
            [
                new UserRole
                {
                    RoleId = roleId,
                    UserId = "user-one",
                    Role = role
                }
            ]
        };

        EventMessage<User> captured = null;

        userEventBrokerMock
            .Setup(expression: broker => broker.RaiseUserAddEventAsync(
                message: It.Is<EventMessage<User>>(match: _ => true)))
            .Callback<EventMessage<User>>(action: message => captured = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseUserAddEventAsync(entity: entity);

        // Then
        captured
            .Should()
            .NotBeNull();

        captured.Data.DefaultCulture
            .Should()
            .BeSameAs(expected: culture);

        captured.Data.Roles
            .Should()
            .ContainSingle()
            .Which.Role
            .Should()
            .BeEquivalentTo(expectation: role);
    }
#pragma warning restore STXFORMAT008
}