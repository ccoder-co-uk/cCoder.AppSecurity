// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Exposures.Controllers;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public partial class PrivilegeControllerTests
{
    [Fact]
    public async Task ShouldReturnBadRequestWhenPutModelIsInvalidAsync()
    {
        // Given
        PrivilegeController controller = CreateController();
        controller.ModelState.AddModelError(key: "Id", errorMessage: "Required");

        // When
        IActionResult result = await controller.Put(
            key: "page_read",
            updatedPrivilege: new Privilege());

        // Then
        result
            .Should()
            .BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPutFailsAsync()
    {
        // Given
        Privilege privilege = new() { Id = "original" };
        PrivilegeController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            privilegeManagerMock.Reset();

            privilegeManagerMock
                .Setup(expression: manager => manager.UpdatePrivilegeAsync(
                    entity: privilege))
                .Throws(exception: exception);

            return controller.Put(key: "page_read", updatedPrivilege: privilege);
        });
    }
}