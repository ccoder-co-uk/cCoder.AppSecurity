// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Exposures.Controllers;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public partial class PrivilegeControllerTests
{
    [Fact]
    public async Task ShouldReturnCreatedPrivilegeWhenPostSucceedsAsync()
    {
        Privilege privilege = new() { Id = "page_read" };

        privilegeManagerMock
            .Setup(expression: manager => manager.AddPrivilegeAsync(entity: privilege))
            .ReturnsAsync(value: privilege);

        IActionResult result = await CreateController().Post(newPrivilege: privilege);

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(201);

        loggingBrokerMock.Verify(
            expression: broker => broker.LogInformation(
                It.IsAny<string>(),
                It.IsAny<object[]>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPostModelIsInvalidAsync()
    {
        // Given
        PrivilegeController controller = CreateController();
        controller.ModelState.AddModelError(key: "Id", errorMessage: "Required");

        // When
        IActionResult result = await controller.Post(newPrivilege: new Privilege());

        // Then
        result
            .Should()
            .BeOfType<BadRequestObjectResult>();

        loggingBrokerMock.Verify(
            expression: broker => broker.LogWarning(
                It.IsAny<string>(),
                It.IsAny<object[]>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPostFailsAsync()
    {
        // Given
        Privilege privilege = new() { Id = "page_read" };
        PrivilegeController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(loggingBrokerMock: loggingBrokerMock, invoke: exception =>
        {
            privilegeManagerMock.Reset();

            privilegeManagerMock
                .Setup(expression: manager => manager.AddPrivilegeAsync(
                    entity: privilege))
                .Throws(exception: exception);

            return controller.Post(newPrivilege: privilege);
        });
    }
}