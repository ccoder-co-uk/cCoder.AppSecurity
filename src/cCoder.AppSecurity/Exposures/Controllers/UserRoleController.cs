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

public sealed partial class UserRoleController(
    IUserRoleOrchestrationService service)
    : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata() =>
        Ok(value: new MetadataContainer(type: typeof(UserRole), isEntity: true, hasEndpoint: true));

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
    public IActionResult GetAll() =>
        Ok(value: service.GetAll());

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserRole newUserRole)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.AddUserRoleAsync(entity: newUserRole));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] IEnumerable<UserRole> deletedUserRole)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(modelState: ModelState);
        }

        await service.DeleteAllUserRoleAsync(items: deletedUserRole);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid keyRoleId, [FromRoute] string keyUserId)
    {
        await service.DeleteUserRoleAsync(entity: new UserRole
        {
            RoleId = keyRoleId,
            UserId = keyUserId,
        });

        return Ok();
    }
}