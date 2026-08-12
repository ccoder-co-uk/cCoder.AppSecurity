// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Exposures.Controllers;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public sealed partial class AppControllerTests
{
    private readonly Mock<IAppManager> appManagerMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetAllFails()
    {
        // Given
        AppController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            appManagerMock.Reset();

            appManagerMock
                .Setup(expression: manager => manager.GetAll())
                .Throws(exception: exception);

            return controller.GetAll();
        });
    }

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetFails()
    {
        // Given
        AppController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            appManagerMock.Reset();

            appManagerMock
                .Setup(expression: manager => manager.GetAll())
                .Throws(exception: exception);

            return controller.Get(key: 7);
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldReturnMetadataWhenGetMetadataSucceeds(bool extended)
    {
        // Given
        AppController controller = CreateController();
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
    public void ShouldReturnAppsWhenGetAll()
    {
        // Given
        App app = new() { Id = 7 };

        appManagerMock
            .Setup(expression: manager => manager.GetAll())
            .Returns(value: new[] { app }.AsQueryable());

        AppController controller = CreateController();

        // When
        IActionResult result = controller.GetAll();

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData(7, true)]
    [InlineData(8, false)]
    public void ShouldReturnExpectedResultWhenGet(int key, bool found)
    {
        // Given
        App app = new() { Id = 7 };

        appManagerMock
            .Setup(expression: manager => manager.GetAll())
            .Returns(value: new[] { app }.AsQueryable());

        AppController controller = CreateController();

        // When
        IActionResult result = controller.Get(key: key);

        // Then
        if (found)
        {
            result
                .Should()
                .BeOfType<OkObjectResult>();
        }
        else
        {
            result
                .Should()
                .BeOfType<NotFoundResult>();
        }
    }

    [Fact]
    public void ShouldReturnInternalServerErrorWhenGetMetadataFails()
    {
        // Given
        AppController controller = CreateController();

        // When
        IActionResult result = controller.GetMetadata();

        // Then
        result
            .Should()
            .BeOfType<StatusCodeResult>()
            .Which.StatusCode
            .Should()
            .Be(expected: StatusCodes.Status500InternalServerError);
    }

    private void AssertExceptionStatusCodes(Func<Exception, IActionResult> invoke)
    {
        (Exception Exception, int StatusCode)[] expectedResults =
        [
            (
                new AppSecurityValidationException(
                    innerException: new ArgumentException()),
                StatusCodes.Status400BadRequest),
            (
                new AppSecurityAuthorizationException(
                    innerException: new UnauthorizedAccessException()),
                StatusCodes.Status403Forbidden),
            (
                new InvalidOperationException(),
                StatusCodes.Status500InternalServerError)
        ];

        foreach ((Exception exception, int statusCode) in expectedResults)
        {
            IActionResult result = invoke(arg: exception);

            result
                .Should()
                .BeAssignableTo<StatusCodeResult>()
                .Which.StatusCode
                .Should()
                .Be(expected: statusCode);
        }
    }

    private AppController CreateController() =>
        new(
            service: appManagerMock.Object,
            loggingBroker: loggingBrokerMock.Object);
}