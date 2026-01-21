namespace SB.Blobs;

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

[ApiExplorerSettings(IgnoreApi = true)]
[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class VersionController
{
    public record VersionResult(string Version, string Environment);

    [HttpGet]
    public ActionResult<VersionResult> GetVersion(
        [FromServices]IWebHostEnvironment webHostEnvironment)
    {
        var version = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        return new VersionResult(version, webHostEnvironment.EnvironmentName);
    }
}
