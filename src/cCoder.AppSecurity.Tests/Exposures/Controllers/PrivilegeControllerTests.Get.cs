// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Moq;
using Xunit;
using cCoder.AppSecurity.Exposures.Controllers;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public partial class PrivilegeControllerTests
{
    [Fact]
    public void ShouldReturnNotFoundWhenGetCannotFindPrivilege()
    {
        // Given
#pragma warning disable STXFORMAT008
        privilegeManagerMock
            .Setup(expression: manager => manager.Get(id: "missing"))
            .Returns(value: null);
#pragma warning restore STXFORMAT008

        PrivilegeController controller = CreateController();

        // When
        IActionResult result = controller.Get(key: "missing");

        // Then
        result
            .Should()
            .BeOfType<NotFoundResult>();
    }

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetFails()
    {
        // Given
        PrivilegeController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            privilegeManagerMock.Reset();

            privilegeManagerMock
                .Setup(expression: manager => manager.Get(id: "page_read"))
                .Throws(exception: exception);

            return controller.Get(key: "page_read");
        });
    }
}