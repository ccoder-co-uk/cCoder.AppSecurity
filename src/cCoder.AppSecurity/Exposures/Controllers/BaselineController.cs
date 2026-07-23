// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.Setup;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.AppSecurity.Exposures.Controllers;

[ApiController]
[Route("Api/AppSecurity/Baseline")]
public sealed class BaselineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(UIBaseline.Packages);
}