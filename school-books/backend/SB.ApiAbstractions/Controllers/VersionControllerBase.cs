namespace SB.ApiAbstractions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

public class VersionControllerBase
{
    [HttpGet]
    public ActionResult<ControllerExtensions.VersionResult> GetVersion(
        [FromServices]IWebHostEnvironment webHostEnvironment)
        => ControllerExtensions.GetVersion(webHostEnvironment);
}
