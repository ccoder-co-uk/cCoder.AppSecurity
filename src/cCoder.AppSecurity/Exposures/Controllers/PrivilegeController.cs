// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Security.Authentication;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Models.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.AppSecurity.Exposures.Controllers;

public sealed partial class PrivilegeController(
    IPrivilegeManager service,
    ILoggingBroker loggingBroker)
    : ODataController
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
            IQueryable<Privilege> result = service.GetAll();
            loggingBroker.LogInformation(message: "Controller request succeeded.");
            return Ok(value: result);
        }
        catch (AppSecurityOrchestrationValidationException exception)
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
    [AllowAnonymous]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    public IActionResult Get([FromRoute] string key)
    {
        try
        {
            Privilege result = service.Get(id: key);

            if (result is null)
            {
                loggingBroker.LogWarning(message: "Requested app security resource was not found.");
                return NotFound();
            }

            loggingBroker.LogInformation(message: "Controller request succeeded.");
            return Ok(value: result);
        }
        catch (AppSecurityOrchestrationValidationException exception)
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

    [HttpPost]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Post([FromBody] Privilege newPrivilege)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "The app security request is invalid.");
                return BadRequest(modelState: ModelState);
            }

            Privilege result = await service.AddPrivilegeAsync(entity: newPrivilege);
            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: result);
        }
        catch (AppSecurityOrchestrationValidationException exception)
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

    [HttpPut]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Put([FromRoute] string key, [FromBody] Privilege updatedPrivilege)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "The app security request is invalid.");
                return BadRequest(modelState: ModelState);
            }

            updatedPrivilege.Id = key;

            Privilege result = await service.UpdatePrivilegeAsync(entity: updatedPrivilege);
            loggingBroker.LogInformation(message: "Controller request succeeded.");
            return Ok(value: result);
        }
        catch (AppSecurityOrchestrationValidationException exception)
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

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        try
        {
            await service.DeleteAsync(id: key);
            loggingBroker.LogInformation(message: "Controller request succeeded.");
            return NoContent();
        }
        catch (AppSecurityOrchestrationValidationException exception)
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