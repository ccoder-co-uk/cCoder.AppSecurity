// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Dependencies.Metadata;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.AppSecurity.Exposures.Controllers;

public sealed partial class UserRoleController(
    IUserRoleManager service)
    : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            return Ok(value: MetadataDependency.CreateMetadataContainer(
                type: typeof(UserRole),
                isEntity: true,
                hasEndpoint: true));
        }
        catch (AppSecurityAuthorizationException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
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
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(value: service.GetAll());
        }
        catch (AppSecurityOrchestrationValidationException)
        {
            return BadRequest();
        }
        catch (AppSecurityAuthorizationException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
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
                return BadRequest(modelState: ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.AddUserRoleAsync(entity: newUserRole));
        }
        catch (AppSecurityOrchestrationValidationException)
        {
            return BadRequest();
        }
        catch (AppSecurityAuthorizationException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
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
                return BadRequest(modelState: ModelState);
            }

            await service.DeleteAllUserRoleAsync(items: deletedUserRole);

            return NoContent();
        }
        catch (AppSecurityOrchestrationValidationException)
        {
            return BadRequest();
        }
        catch (AppSecurityAuthorizationException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
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

            return NoContent();
        }
        catch (AppSecurityOrchestrationValidationException)
        {
            return BadRequest();
        }
        catch (AppSecurityAuthorizationException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}