// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using cCoder.AppSecurity.Exposures.Controllers;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public partial class PrivilegeControllerTests
{
    [Fact]
    public void ShouldReturnPrivilegesWhenGetAll()
    {
        Privilege privilege = new() { Id = "page_read" };

        privilegeManagerMock
            .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
            .Returns(value: new[] { privilege }.AsQueryable());

        IActionResult result = CreateController().GetAll();

        result.Should().BeOfType<OkObjectResult>();

        loggingBrokerMock.Verify(
            expression: broker => broker.LogInformation(
                It.IsAny<string>(),
                It.IsAny<object[]>()),
            times: Times.Once);
    }

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetAllFails()
    {
        // Given
        PrivilegeController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(loggingBrokerMock: loggingBrokerMock, invoke: exception =>
        {
            privilegeManagerMock.Reset();

            privilegeManagerMock
                .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
                .Throws(exception: exception);

            return controller.GetAll();
        });
    }
}