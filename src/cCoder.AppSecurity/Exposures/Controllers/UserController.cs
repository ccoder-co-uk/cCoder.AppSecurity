// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Dependencies.Metadata;
using cCoder.AppSecurity.Brokers.OData;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Extensions;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.AppSecurity.Exposures.Controllers;

public sealed partial class UserController(
    IUserManager service,
    ICoreAuthInfo authInfo)
    : ODataController
{
    [HttpGet]
    [ActionName("Me")]
    public IActionResult GetMe()
    {
        try
        {
            User user = service.Get(id: authInfo.SSOUserId);

            return user is null
                ? NotFound()
                : Ok(value: user);
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

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            bool isExtendedMetaRequest = Request.Query["extend"] == "true";

            return isExtendedMetaRequest
                ? Ok(value: new AppSecurityODataModelBroker()
                    .SelectODataModel()
                    .EDMModel.GetExtendedMetadataForType(
                        context: "AppSecurity",
                        type: typeof(User)))
                : Ok(value: MetadataDependency.CreateMetadataContainer(
                    type: typeof(User),
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
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
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
            User result = service.Get(id: key);

            return result is null
                ? NotFound()
                : Ok(value: result);
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
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Post([FromBody] User newUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.AddUserAsync(entity: newUser));
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

    [HttpPut]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Put([FromRoute] string key, [FromBody] User updatedUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            updatedUser.Id = key;

            return Ok(value: await service.UpdateUserAsync(entity: updatedUser));
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

    [AcceptVerbs("PATCH", "MERGE")]
    [ActionName("Patch")]
    public async Task<IActionResult> Put([FromRoute] string key, Delta<User> updatedDelta)
    {
        try
        {
            User originalEntity = service.Get(id: key);

            if (originalEntity is null)
            {
                return NotFound();
            }

            updatedDelta.Patch(original: originalEntity);

            return Ok(value: await service.UpdateUserAsync(entity: originalEntity));
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
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        try
        {
            await service.DeleteAsync(id: key);

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