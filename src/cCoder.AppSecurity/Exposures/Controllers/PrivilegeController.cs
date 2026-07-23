// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Brokers.Metadata;
using cCoder.AppSecurity.Brokers.OData;
using cCoder.AppSecurity.Models;
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
    IPrivilegeOrchestrationService service)
    : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
value: new AppSecurityODataModelBroker()
                    .SelectODataModel()
                    .EDMModel.GetExtendedMetadataForType(context: "AppSecurity", type: typeof(Privilege))
            )
            : Ok(
                value: MetadataBroker.CreateMetadataContainer(
                    type: typeof(Privilege),
                    isEntity: true,
                    hasEndpoint: true));
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
    public IActionResult GetAll(ODataQueryOptions<Privilege> queryOptions) =>
        Ok(value: service.GetAll());

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
            IQueryable<Privilege> result = service.GetAll()
                .Where(predicate: privilege => privilege.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
        }
        catch (System.Security.SecurityException)
        {
            return NotFound();
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
        if (!ModelState.IsValid)
        {
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.AddPrivilegeAsync(entity: newPrivilege));
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
        if (!ModelState.IsValid)
        {
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.UpdatePrivilegeAsync(entity: updatedPrivilege));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Put([FromRoute] string key, Delta<Privilege> updatedDelta)
    {
        Privilege originalEntity = service.Get(id: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        updatedDelta.Patch(original: originalEntity);
        return Ok(value: await service.UpdatePrivilegeAsync(entity: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        await service.DeleteAsync(id: key);
        return Ok();
    }
}