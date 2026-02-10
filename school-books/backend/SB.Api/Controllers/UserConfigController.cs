namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IUserConfigQueryRepository;

[Authorize(Policy = Policies.AuthenticatedAccess)]
[ApiController]
[Route("api/[controller]")]
public class UserConfigController
{
    [HttpGet]
    public async Task<ActionResult<GetVO>> GetUserConfigAsync(
        [FromServices] IUserConfigQueryRepository userConfigQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await userConfigQueryRepository.GetAsync(
            httpContextAccessor.GetInstId(),
            ct);
}
