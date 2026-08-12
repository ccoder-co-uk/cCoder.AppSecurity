// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public async Task ShouldReturnNoContentWhenDeleteSucceedsAsync()
    {
        // Given
#pragma warning disable STXFORMAT008
        privilegeManagerMock
            .Setup(expression: manager => manager.DeleteAsync(id: "page_read"))
            .Returns(value: ValueTask.CompletedTask);
#pragma warning restore STXFORMAT008

        PrivilegeController controller = CreateController();

        // When
        IActionResult result = await controller.Delete(key: "page_read");

        // Then
        result
            .Should()
            .BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenDeleteFailsAsync()
    {
        // Given
        PrivilegeController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            privilegeManagerMock.Reset();

            privilegeManagerMock
                .Setup(expression: manager => manager.DeleteAsync(id: "page_read"))
                .Throws(exception: exception);

            return controller.Delete(key: "page_read");
        });
    }
}