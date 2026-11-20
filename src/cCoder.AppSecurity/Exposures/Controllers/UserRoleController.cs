// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Models.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.AppSecurity.Exposures.Controllers;

public sealed partial class UserRoleController(
    IUserRoleManager service,
    ILoggingBroker loggingBroker)
    : ODataController
{
    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {
            IQueryable<UserRole> result = service.GetAll();
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
    public async Task<IActionResult> Post([FromBody] UserRole newUserRole)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "The app security request is invalid.");
                return BadRequest(modelState: ModelState);
            }

            UserRole result = await service.AddUserRoleAsync(entity: newUserRole);
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

    [HttpDelete]
    public async Task<IActionResult> DeleteAll([FromBody] IEnumerable<UserRole> deletedUserRole)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "The app security request is invalid.");
                return BadRequest(modelState: ModelState);
            }

            await service.DeleteAllUserRoleAsync(items: deletedUserRole);
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

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid keyRoleId, [FromRoute] string keyUserId)
    {
        try
        {

            await service.DeleteUserRoleAsync(entity: new UserRole
            {
                RoleId = keyRoleId,
                UserId = keyUserId,
            });

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