namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Data;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class TopicPlanNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromRoute][SuppressMessage("", "IDE0060")]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromQuery][NotNull] int[] ids,
        [FromServices] ITopicPlanNomsRepository repository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(
            httpContextAccessor.GetSysUserId(),
            ids,
            ct);

    [HttpGet]
    public async Task<NomVO[]> GetNomsByTermAsync(
        [FromRoute][SuppressMessage("", "IDE0060")]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ITopicPlanNomsRepository repository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(
            httpContextAccessor.GetSysUserId(),
            term,
            offset,
            limit,
            ct);
}
