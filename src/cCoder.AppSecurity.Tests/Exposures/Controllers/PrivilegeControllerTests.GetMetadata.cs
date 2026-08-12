// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public partial class PrivilegeControllerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldReturnMetadataWhenGetMetadataSucceeds(bool extended)
    {
        // Given
        PrivilegeController controller = CreateController();
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
}