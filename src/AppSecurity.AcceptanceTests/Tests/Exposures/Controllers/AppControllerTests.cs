// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Security.Authentication;
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

            loggingBrokerMock.Verify(
                expression: broker => broker.LogWarning(
                    It.IsAny<string>(),
                    It.IsAny<object[]>()),
                times: Times.Once);
        }
    }

    private void AssertExceptionStatusCodes(Func<Exception, IActionResult> invoke)
    {
        (Exception Exception, int StatusCode)[] expectedResults =
        [
            (
                new AuthenticationException(message: "Authentication failed."),
                StatusCodes.Status401Unauthorized),
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

            int? actualStatusCode = result switch
            {
                ObjectResult objectResult => objectResult.StatusCode,
                StatusCodeResult statusCodeResult => statusCodeResult.StatusCode,
                _ => null
            };

            actualStatusCode.Should().Be(expected: statusCode);

            if (statusCode == StatusCodes.Status401Unauthorized)
            {
                result.Should().BeOfType<UnauthorizedObjectResult>()
                    .Which.Value.Should().Be("Authentication failed.");
            }

            Times warningTimes = statusCode < StatusCodes.Status500InternalServerError
                ? Times.Once()
                : Times.Never();

            Times errorTimes = statusCode == StatusCodes.Status500InternalServerError
                ? Times.Once()
                : Times.Never();

            loggingBrokerMock.Verify(
                expression: broker => broker.LogWarning(
                    It.IsAny<Exception>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>()),
                times: warningTimes);

            loggingBrokerMock.Verify(
                expression: broker => broker.LogError(
                    It.IsAny<Exception>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>()),
                times: errorTimes);

            loggingBrokerMock.Verify(
                expression: broker => broker.LogInformation(
                    It.IsAny<string>(),
                    It.IsAny<object[]>()),
                times: Times.Never);

            loggingBrokerMock.Reset();
        }
    }

    private AppController CreateController() =>
        new(
            service: appManagerMock.Object,
            loggingBroker: loggingBrokerMock.Object);
}