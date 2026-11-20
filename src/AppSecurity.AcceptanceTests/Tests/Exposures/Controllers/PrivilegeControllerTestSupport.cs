// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

internal static class PrivilegeControllerTestSupport
{
    internal static void AssertExceptionStatusCodes(
        Mock<ILoggingBroker> loggingBrokerMock,
        Func<Exception, IActionResult> invoke,
        bool includeValidation = true)
    {
        var expectedResults = new List<(Exception Exception, int StatusCode)>();

        if (includeValidation)
        {
            expectedResults.Add(item: (
                new AppSecurityOrchestrationValidationException(
                    innerException: new ArgumentException()),
                StatusCodes.Status400BadRequest));
        }

        expectedResults.Add(item: (
            new AuthenticationException(message: "Authentication failed."),
            StatusCodes.Status401Unauthorized));

        expectedResults.Add(item: (
            new AppSecurityAuthorizationException(
                innerException: new UnauthorizedAccessException()),
            StatusCodes.Status403Forbidden));

        expectedResults.Add(item: (
            new InvalidOperationException(),
            StatusCodes.Status500InternalServerError));

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

            VerifyExpectedLog(loggingBrokerMock, statusCode);
        }
    }

    internal static async Task AssertExceptionStatusCodesAsync(
        Mock<ILoggingBroker> loggingBrokerMock,
        Func<Exception, Task<IActionResult>> invoke)
    {
        (Exception Exception, int StatusCode)[] expectedResults =
        [
            (
                new AuthenticationException(message: "Authentication failed."),
                StatusCodes.Status401Unauthorized),
            (
                new AppSecurityOrchestrationValidationException(
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
            IActionResult result = await invoke(arg: exception);

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

            VerifyExpectedLog(loggingBrokerMock, statusCode);
        }
    }

    private static void VerifyExpectedLog(
        Mock<ILoggingBroker> loggingBrokerMock,
        int statusCode)
    {
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