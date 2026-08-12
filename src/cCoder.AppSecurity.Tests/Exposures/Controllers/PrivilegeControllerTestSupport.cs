// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

internal static class PrivilegeControllerTestSupport
{
    internal static void AssertExceptionStatusCodes(
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
            new AppSecurityAuthorizationException(
                innerException: new UnauthorizedAccessException()),
            StatusCodes.Status403Forbidden));

        expectedResults.Add(item: (
            new InvalidOperationException(),
            StatusCodes.Status500InternalServerError));

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

    internal static async Task AssertExceptionStatusCodesAsync(
        Func<Exception, Task<IActionResult>> invoke)
    {
        (Exception Exception, int StatusCode)[] expectedResults =
        [
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

            result
                .Should()
                .BeAssignableTo<StatusCodeResult>()
                .Which.StatusCode
                .Should()
                .Be(expected: statusCode);
        }
    }
}