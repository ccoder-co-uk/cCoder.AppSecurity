// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Models.CMS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.AppSecurity.Exposures.Controllers;

public sealed class AppController(IAppManager service, ILoggingBroker loggingBroker) : ODataController
{
    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {
            IQueryable<App> result = service.GetAll();
            loggingBroker.LogInformation(message: "Controller request succeeded.");
            return Ok(value: result);
        }
        catch (AppSecurityValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "Controller request was rejected.");

            return BadRequest();
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "Controller request was rejected.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (AppSecurityAuthorizationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "Controller request was rejected.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    public IActionResult Get([FromRoute] int key)
    {
        try
        {
            App result = service.GetAll()
                .FirstOrDefault(predicate: app => app.Id == key);

            if (result is null)
            {
                loggingBroker.LogWarning(message: "Requested app security resource was not found.");
                return NotFound();
            }

            loggingBroker.LogInformation(message: "Controller request succeeded.");
            return Ok(value: result);
        }
        catch (AppSecurityValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "Controller request was rejected.");

            return BadRequest();
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "Controller request was rejected.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (AppSecurityAuthorizationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "Controller request was rejected.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}