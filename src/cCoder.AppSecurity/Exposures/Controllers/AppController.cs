// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Dependencies.Metadata;
using cCoder.AppSecurity.Brokers.OData;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.AppSecurity.Exposures.Controllers;

public sealed class AppController(IAppManager service) : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            bool isExtendedMetaRequest = Request.Query["extend"] == "true";

            return isExtendedMetaRequest
                ? Ok(
                    value: new AppSecurityODataModelBroker()
                        .SelectODataModel()
                        .EDMModel.GetExtendedMetadataForType(
                            context: "AppSecurity",
                            type: typeof(App)))
                : Ok(
                    value: MetadataDependency.CreateMetadataContainer(
                        type: typeof(App),
                        isEntity: true,
                        hasEndpoint: false));
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
        catch (AppSecurityValidationException)
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

            return result is null
                ? NotFound()
                : Ok(value: result);
        }
        catch (AppSecurityValidationException)
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