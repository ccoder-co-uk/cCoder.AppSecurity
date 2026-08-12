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

public sealed partial class UserControllerExceptionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldReturnMetadataWhenGetMetadata(bool extended)
    {
        // Given
        UserController controller = CreateController();

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
    public void ShouldReturnUserWhenGetMe()
    {
        // Given
        User user = new() { Id = "user-one" };

#pragma warning disable STXFORMAT008
        authInfoMock
            .SetupGet(expression: auth => auth.SSOUserId)
            .Returns(value: user.Id);
#pragma warning restore STXFORMAT008

        userManagerMock
            .Setup(expression: manager => manager.Get(id: user.Id))
            .Returns(value: user);

        UserController controller = CreateController();

        // When
        IActionResult result = controller.GetMe();

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which.Value
            .Should()
            .BeSameAs(expected: user);
    }

    [Fact]
    public void ShouldReturnNotFoundWhenGetMeHasNoUser()
    {
        // Given
#pragma warning disable STXFORMAT008
        authInfoMock
            .SetupGet(expression: auth => auth.SSOUserId)
            .Returns(value: "missing");
#pragma warning restore STXFORMAT008

        userManagerMock
            .Setup(expression: manager => manager.Get(id: "missing"))
            .Returns(value: null);

        UserController controller = CreateController();

        // When
        IActionResult result = controller.GetMe();

        // Then
        result
            .Should()
            .BeOfType<NotFoundResult>();
    }

    [Fact]
    public void ShouldReturnUsersWhenGetAll()
    {
        // Given
        User user = new() { Id = "user-one" };

        userManagerMock
            .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
            .Returns(value: new[] { user }.AsQueryable());

        UserController controller = CreateController();

        // When
        IActionResult result = controller.GetAll();

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnUserWhenGet()
    {
        // Given
        User user = new() { Id = "user-one" };

        userManagerMock
            .Setup(expression: manager => manager.Get(id: user.Id))
            .Returns(value: user);

        UserController controller = CreateController();

        // When
        IActionResult result = controller.Get(key: user.Id);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnCreatedUserWhenPostAsync()
    {
        // Given
        User user = new() { Id = "user-one" };

        userManagerMock
            .Setup(expression: manager => manager.AddUserAsync(entity: user))
            .ReturnsAsync(value: user);

        UserController controller = CreateController();

        // When
        IActionResult result = await controller.Post(newUser: user);

        // Then
        result
            .Should()
            .BeOfType<ObjectResult>()
            .Which.StatusCode
            .Should()
            .Be(expected: 201);
    }

    [Fact]
    public async Task ShouldReturnUpdatedUserWhenPutAsync()
    {
        // Given
        User user = new();

        userManagerMock
            .Setup(expression: manager => manager.UpdateUserAsync(entity: user))
            .ReturnsAsync(value: user);

        UserController controller = CreateController();

        // When
        IActionResult result = await controller.Put(
            key: "user-one",
            updatedUser: user);

        // Then
        user.Id
            .Should()
            .Be(expected: "user-one");

        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNotFoundWhenPatchHasNoUserAsync()
    {
        // Given
        userManagerMock
            .Setup(expression: manager => manager.Get(id: "missing"))
            .Returns(value: null);

        UserController controller = CreateController();

        // When
        IActionResult result = await controller.Put(
            key: "missing",
            updatedDelta: new Delta<User>());

        // Then
        result
            .Should()
            .BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteAsync()
    {
        // Given
        userManagerMock
            .Setup(expression: manager => manager.DeleteAsync(id: "user-one"))
            .Returns(value: ValueTask.CompletedTask);

        UserController controller = CreateController();

        // When
        IActionResult result = await controller.Delete(key: "user-one");

        // Then
        result
            .Should()
            .BeOfType<NoContentResult>();
    }
}