// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPostFailsAsync()
    {
        // Given
        Privilege privilege = new() { Id = "page_read" };
        PrivilegeController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
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