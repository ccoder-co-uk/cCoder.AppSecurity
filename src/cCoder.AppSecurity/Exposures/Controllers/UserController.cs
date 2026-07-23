// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.AppSecurity.Exposures.Controllers;

public partial class UserController : ODataController
{
    protected IUserOrchestrationService Service { get; }
    private readonly ICoreAuthInfo authInfo;

    public UserController(
        IUserOrchestrationService service,
        ICoreAuthInfo auth,
        ILogger<UserController> log
    )
    {
        Service = service;
        authInfo = auth;
    }

    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.All,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 6,
        MaxExpansionDepth = 6
    )]
    public IActionResult Me() =>
        Ok(value: Service.Get(id: authInfo.SSOUserId));

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
value: new cCoder.AppSecurity.Api.OData.AppSecurityModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType(context: "AppSecurity", type: typeof(User))
            )
            : Ok(value: new MetadataContainer(type: typeof(User), isEntity: true, hasEndpoint: true));
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
    public IActionResult GetAll(ODataQueryOptions<User> queryOptions) =>
        Ok(value: Service.GetAll());

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
            IQueryable<User> result = Service.GetAll()
                .Where(predicate: user => user.Id == key);

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
    public async Task<IActionResult> Post([FromBody] User newUser)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await Service.AddUserAsync(entity: newUser));
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
        if (!ModelState.IsValid)
        {
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await Service.UpdateUserAsync(entity: updatedUser));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] string key, Delta<User> delta)
    {
        User originalEntity = Service.Get(id: key);
        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await Service.UpdateUserAsync(entity: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        await Service.DeleteAsync(id: key);
        return Ok();
    }
}