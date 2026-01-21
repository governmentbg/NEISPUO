namespace SB.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = Policies.AuthenticatedAccess)]
[ApiController]
[Route("api/[controller]/[action]")]
public abstract class CommonNomsController
{
}
