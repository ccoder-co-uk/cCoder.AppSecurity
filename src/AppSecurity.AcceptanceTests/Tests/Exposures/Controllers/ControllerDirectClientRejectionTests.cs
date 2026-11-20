// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Exposures.Controllers;
using cCoder.Data;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public sealed class ControllerDirectClientRejectionTests
{
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    [Fact]
    public async Task ShouldWarnWhenRolePostModelIsInvalidAsync()
    {
        var controller = new RoleController(
            service: Mock.Of<IRoleManager>(),
            loggingBroker: loggingBrokerMock.Object);

        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Post(newRole: new Role());

        AssertBadRequestWarning(result);
    }

    [Fact]
    public async Task ShouldWarnWhenRolePutModelIsInvalidAsync()
    {
        var controller = new RoleController(
            service: Mock.Of<IRoleManager>(),
            loggingBroker: loggingBrokerMock.Object);

        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Put(
            key: Guid.NewGuid(),
            updatedRole: new Role());

        AssertBadRequestWarning(result);
    }

    [Fact]
    public void ShouldWarnWhenUserGetCannotFindUser()
    {
        var userManagerMock = new Mock<IUserManager>();

        var controller = new UserController(
            service: userManagerMock.Object,
            authInfo: Mock.Of<ICoreAuthInfo>(),
            loggingBroker: loggingBrokerMock.Object);

        IActionResult result = controller.Get(key: "missing");

        result.Should().BeOfType<NotFoundResult>();
        VerifySingleWarning();
    }

    [Fact]
    public async Task ShouldWarnWhenUserPostModelIsInvalidAsync()
    {
        var controller = new UserController(
            service: Mock.Of<IUserManager>(),
            authInfo: Mock.Of<ICoreAuthInfo>(),
            loggingBroker: loggingBrokerMock.Object);

        controller.ModelState.AddModelError(key: "Id", errorMessage: "Required");

        IActionResult result = await controller.Post(newUser: new User());

        AssertBadRequestWarning(result);
    }

    [Fact]
    public async Task ShouldWarnWhenUserPutModelIsInvalidAsync()
    {
        var controller = new UserController(
            service: Mock.Of<IUserManager>(),
            authInfo: Mock.Of<ICoreAuthInfo>(),
            loggingBroker: loggingBrokerMock.Object);

        controller.ModelState.AddModelError(key: "Id", errorMessage: "Required");

        IActionResult result = await controller.Put(key: "user", updatedUser: new User());

        AssertBadRequestWarning(result);
    }

    [Fact]
    public async Task ShouldWarnWhenUserRolePostModelIsInvalidAsync()
    {
        var controller = new UserRoleController(
            service: Mock.Of<IUserRoleManager>(),
            loggingBroker: loggingBrokerMock.Object);

        controller.ModelState.AddModelError(key: "UserId", errorMessage: "Required");

        IActionResult result = await controller.Post(newUserRole: new UserRole());

        AssertBadRequestWarning(result);
    }

    [Fact]
    public async Task ShouldWarnWhenUserRoleDeleteAllModelIsInvalidAsync()
    {
        var controller = new UserRoleController(
            service: Mock.Of<IUserRoleManager>(),
            loggingBroker: loggingBrokerMock.Object);

        controller.ModelState.AddModelError(key: "UserId", errorMessage: "Required");

        IActionResult result = await controller.DeleteAll(deletedUserRole: []);

        AssertBadRequestWarning(result);
    }

    private void AssertBadRequestWarning(IActionResult result)
    {
        result.Should().BeOfType<BadRequestObjectResult>();
        VerifySingleWarning();
    }

    private void VerifySingleWarning()
    {
        loggingBrokerMock.Verify(
            expression: broker => broker.LogWarning(
                It.IsAny<string>(),
                It.IsAny<object[]>()),
            times: Times.Once);

        loggingBrokerMock.Verify(
            expression: broker => broker.LogInformation(
                It.IsAny<string>(),
                It.IsAny<object[]>()),
            times: Times.Never);

        loggingBrokerMock.Verify(
            expression: broker => broker.LogError(
                It.IsAny<Exception>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()),
            times: Times.Never);
    }
}
