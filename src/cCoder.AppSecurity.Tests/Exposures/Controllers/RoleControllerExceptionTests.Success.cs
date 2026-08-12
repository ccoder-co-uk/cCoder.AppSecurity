// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.Controllers;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public sealed partial class RoleControllerExceptionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldReturnMetadataWhenGetMetadata(bool extended)
    {
        // Given
        RoleController controller = CreateController();

        controller.ControllerContext = new ControllerContext();

        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        controller.Request.QueryString = extended
            ? new QueryString(value: "?extend=true")
            : QueryString.Empty;

        // When
        IActionResult result = controller.GetMetadata();

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnRolesWhenGetAll()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };

        roleManagerMock
            .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
            .Returns(value: new[] { role }.AsQueryable());

        RoleController controller = CreateController();

        // When
        IActionResult result = controller.GetAll();

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnRoleWhenGet()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };

        roleManagerMock
            .Setup(expression: manager => manager.Get(id: role.Id))
            .Returns(value: role);

        RoleController controller = CreateController();

        // When
        IActionResult result = controller.Get(key: role.Id);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnNotFoundWhenGetHasNoRole()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        roleManagerMock
            .Setup(expression: manager => manager.Get(id: roleId))
            .Returns(value: null);

        RoleController controller = CreateController();

        // When
        IActionResult result = controller.Get(key: roleId);

        // Then
        result
            .Should()
            .BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldReturnCreatedRoleWhenPostAsync()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };

        roleManagerMock
            .Setup(expression: manager => manager.AddRoleAsync(entity: role))
            .ReturnsAsync(value: role);

        RoleController controller = CreateController();

        // When
        IActionResult result = await controller.Post(newRole: role);

        // Then
        result
            .Should()
            .BeOfType<ObjectResult>()
            .Which.StatusCode
            .Should()
            .Be(expected: 201);
    }

    [Fact]
    public async Task ShouldReturnUpdatedRoleWhenPutAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        Role role = new();

        roleManagerMock
            .Setup(expression: manager => manager.UpdateRoleAsync(entity: role))
            .ReturnsAsync(value: role);

        RoleController controller = CreateController();

        // When
        IActionResult result = await controller.Put(
            key: roleId,
            updatedRole: role);

        // Then
        role.Id
            .Should()
            .Be(expected: roleId);

        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNotFoundWhenPatchHasNoRoleAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        roleManagerMock
            .Setup(expression: manager => manager.Get(id: roleId))
            .Returns(value: null);

        RoleController controller = CreateController();

        // When
        IActionResult result = await controller.Put(
            key: roleId,
            updatedDelta: new Delta<Role>());

        // Then
        result
            .Should()
            .BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        roleManagerMock
            .Setup(expression: manager => manager.DeleteAsync(id: roleId))
            .Returns(value: ValueTask.CompletedTask);

        RoleController controller = CreateController();

        // When
        IActionResult result = await controller.Delete(key: roleId);

        // Then
        result
            .Should()
            .BeOfType<NoContentResult>();
    }
}