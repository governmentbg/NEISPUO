namespace SB.ExtApi;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.ApiAbstractions;

[ApiExplorerSettings(IgnoreApi = true)]
[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class VersionController : VersionControllerBase
{
}
