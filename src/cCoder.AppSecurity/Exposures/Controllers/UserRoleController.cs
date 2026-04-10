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

    public UserRoleController(
        IUserRoleOrchestrationService service,
        ILogger<UserRoleController> log
    )
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata() => Ok(new MetadataContainer(typeof(UserRole), true, true));

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
    public IActionResult GetAll() => Ok(Service.GetAll());

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserRole entity)
    {
        if (!ModelState.IsValid)
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(ModelState);

        return Ok(await Service.AddAsync(entity));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] ODataCollection<UserRole> items)
    {
        if (!ModelState.IsValid)
            return new cCoder.AppSecurity.Api.OData.BadRequestResult(ModelState);

        await Service.DeleteAllAsync(items.Value);
        return Ok();
    }
}
















