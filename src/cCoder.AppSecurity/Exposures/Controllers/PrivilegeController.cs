// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Dependencies.Metadata;
using cCoder.AppSecurity.Brokers.OData;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Extensions;
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

public sealed partial class PrivilegeController(
    IPrivilegeManager service)
    : ODataController
{
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
                        type: typeof(Privilege)))
                : Ok(value: MetadataDependency.CreateMetadataContainer(
                    type: typeof(Privilege),
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
            Privilege result = service.Get(id: key);

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
    public async Task<IActionResult> Post([FromBody] Privilege newPrivilege)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.AddPrivilegeAsync(entity: newPrivilege));
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
    public async Task<IActionResult> Put([FromRoute] string key, [FromBody] Privilege updatedPrivilege)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            updatedPrivilege.Id = key;

            return Ok(value: await service.UpdatePrivilegeAsync(entity: updatedPrivilege));
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
    public async Task<IActionResult> Put([FromRoute] string key, Delta<Privilege> updatedDelta)
    {
        try
        {
            Privilege originalEntity = service.Get(id: key);

            if (originalEntity is null)
            {
                return NotFound();
            }

            updatedDelta.Patch(original: originalEntity);

            return Ok(value: await service.UpdatePrivilegeAsync(entity: originalEntity));
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