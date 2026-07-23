// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.AppSecurity.Exposures.Controllers;

[ApiController]
[Route("Api/AppSecurity/Baseline")]
public sealed class BaselineController(IUIBaselineService baselineService)
    : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: baselineService.GetPackages());
}