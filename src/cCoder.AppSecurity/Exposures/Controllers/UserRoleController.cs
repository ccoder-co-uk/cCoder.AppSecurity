// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.AppSecurity.Exposures.Controllers;

public partial class UserRoleController : ODataController
{
    protected IUserRoleOrchestrationService Service { get; }

    public UserRoleController(IUserRoleOrchestrationService service)
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata() => Ok(value: new MetadataContainer(typeof(UserRole), true, true));

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
    public IActionResult GetAll() => Ok(value: Service.GetAll());

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserRole entity)
    {
        if (!ModelState.IsValid)
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(modelState: ModelState);

        return Ok(value: await Service.AddAsync(entity));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] ODataCollection<UserRole> items)
    {
        if (!ModelState.IsValid)
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(modelState: ModelState);

        await Service.DeleteAllAsync(items: items.Value);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid keyRoleId, [FromRoute] string keyUserId)
    {
        await Service.DeleteAsync(entity: new UserRole
        {
            RoleId = keyRoleId,
            UserId = keyUserId,
        });

        return Ok();
    }
}