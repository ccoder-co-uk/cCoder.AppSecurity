// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.Controllers;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public sealed partial class UserRoleControllerExceptionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldReturnMetadataWhenGetMetadata(bool extended)
    {
        // Given
        UserRoleController controller = CreateController();

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
    public void ShouldReturnUserRolesWhenGetAll()
    {
        // Given
        UserRole userRole = new() { RoleId = Guid.NewGuid(), UserId = "user-one" };

        userRoleManagerMock
            .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
            .Returns(value: new[] { userRole }.AsQueryable());

        UserRoleController controller = CreateController();

        // When
        IActionResult result = controller.GetAll();

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnCreatedUserRoleWhenPostAsync()
    {
        // Given
        UserRole userRole = new() { RoleId = Guid.NewGuid(), UserId = "user-one" };

        userRoleManagerMock
            .Setup(expression: manager => manager.AddUserRoleAsync(entity: userRole))
            .ReturnsAsync(value: userRole);

        UserRoleController controller = CreateController();

        // When
        IActionResult result = await controller.Post(newUserRole: userRole);

        // Then
        result
            .Should()
            .BeOfType<ObjectResult>()
            .Which.StatusCode
            .Should()
            .Be(expected: 201);
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteAllAsync()
    {
        // Given
        UserRole[] userRoles =
        [
            new() { RoleId = Guid.NewGuid(), UserId = "user-one" }
        ];

        userRoleManagerMock
            .Setup(expression: manager => manager.DeleteAllUserRoleAsync(
                items: userRoles))
            .Returns(value: ValueTask.CompletedTask);

        UserRoleController controller = CreateController();

        // When
        IActionResult result = await controller.DeleteAll(
            deletedUserRole: userRoles);

        // Then
        result
            .Should()
            .BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        userRoleManagerMock
            .Setup(expression: manager => manager.DeleteUserRoleAsync(
                entity: It.Is<UserRole>(match: userRole =>
                    userRole.RoleId == roleId
                    && userRole.UserId == "user-one")))
            .Returns(value: ValueTask.CompletedTask);

        UserRoleController controller = CreateController();

        // When
        IActionResult result = await controller.Delete(
            keyRoleId: roleId,
            keyUserId: "user-one");

        // Then
        result
            .Should()
            .BeOfType<NoContentResult>();
    }
}