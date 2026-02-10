namespace SB.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.ApiAbstractions;

[ApiExplorerSettings(IgnoreApi = true)]
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class VersionController : VersionControllerBase
{
}
