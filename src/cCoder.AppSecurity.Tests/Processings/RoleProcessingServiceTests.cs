// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    private User currentUser = WithoutPrivileges();
    private readonly Mock<IRoleService> roleServiceMock = new();
    private readonly RoleProcessingService roleProcessingService;

    public RoleProcessingServiceTests()
    {
        roleProcessingService = new RoleProcessingService(
            service: roleServiceMock.Object);
    }

    [Fact]
    public void ShouldReturnRolesWhenGetAll()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };

        roleServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Returns(value: new[] { role }.AsQueryable());

        RoleProcessingService service = CreateService();

        // When
        IQueryable<Role> result = service.GetAll();

        // Then
        result
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeSameAs(expected: role);
    }

    [Fact]
    public async Task ShouldReturnRoleWhenAddValidatedRoleAsync()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };

        roleServiceMock
            .Setup(expression: service => service.AddValidatedRoleAsync(role: role))
            .ReturnsAsync(value: role);

        RoleProcessingService service = CreateService();

        // When
        Role result = await service.AddValidatedRoleAsync(newRole: role);

        // Then
        result
            .Should()
            .BeSameAs(expected: role);
    }

    [Fact]
    public async Task ShouldReturnRoleWhenUpdateRoleAsync()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };

        roleServiceMock
            .Setup(expression: service => service.UpdateRoleAsync(role: role))
            .ReturnsAsync(value: role);

        RoleProcessingService service = CreateService();

        // When
        Role result = await service.UpdateRoleAsync(updatedRole: role);

        // Then
        result
            .Should()
            .BeSameAs(expected: role);
    }

    [Fact]
    public async Task ShouldReturnRoleWhenUpdateValidatedRoleAsync()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };

        roleServiceMock
            .Setup(expression: service => service.UpdateValidatedRoleAsync(role: role))
            .ReturnsAsync(value: role);

        RoleProcessingService service = CreateService();

        // When
        Role result = await service.UpdateValidatedRoleAsync(updatedRole: role);

        // Then
        result
            .Should()
            .BeSameAs(expected: role);
    }

    [Fact]
    public async Task ShouldDeleteRoleWhenDeleteValidatedAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        roleServiceMock
            .Setup(expression: service => service.DeleteValidatedAsync(id: roleId))
            .Returns(value: ValueTask.CompletedTask);

        RoleProcessingService service = CreateService();

        // When
        await service.DeleteValidatedAsync(roleId: roleId);

        // Then
        roleServiceMock.Verify(
            expression: foundation => foundation.DeleteValidatedAsync(id: roleId),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldDeleteEveryRoleWhenDeleteAllRoleAsync()
    {
        // Given
        Role firstRole = new() { Id = Guid.NewGuid() };
        Role secondRole = new() { Id = Guid.NewGuid() };

        roleServiceMock
            .Setup(expression: service => service.DeleteAsync(id: firstRole.Id))
            .Returns(value: ValueTask.CompletedTask);

        roleServiceMock
            .Setup(expression: service => service.DeleteAsync(id: secondRole.Id))
            .Returns(value: ValueTask.CompletedTask);

        RoleProcessingService service = CreateService();

        // When
        await service.DeleteAllRoleAsync(
            deletedRole: new[] { firstRole, secondRole });

        // Then
        roleServiceMock.Verify(
            expression: foundation => foundation.DeleteAsync(
                id: It.IsAny<Guid>()),
            times: Times.Exactly(callCount: 2));
    }

    private RoleProcessingService CreateService() =>
        new(service: roleServiceMock.Object);

    private static User WithoutPrivileges() =>
        new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = [],
        };

    private static Role CreateRandomRole() =>
        Builder<Role>
            .CreateNew()
            .With(func: x => x.Id = Guid.NewGuid())
            .With(func: x => x.AppId = 1)
            .With(func: x => x.Name = $"Role-{Guid.NewGuid():N}")
            .With(func: x => x.Privs = "app_read")
            .With(func: x => x.App = null)
            .With(func: x => x.Users = [])
            .With(func: x => x.Pages = [])
            .With(func: x => x.Folders = [])
            .Build();
}