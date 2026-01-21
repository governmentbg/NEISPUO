namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Data;
using SB.Domain;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class TopicPlanPublisherNomsController
{
    [HttpGet]
    public async Task<NomVO[]> GetNomsByIdAsync(
        [FromRoute][SuppressMessage("", "IDE0060")]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromQuery][NotNull] int[] ids,
        [FromServices] IEntityNomsRepository<TopicPlanPublisher> repository,
        CancellationToken ct)
        => await repository.GetNomsByIdAsync(ids, ct);

    [HttpGet]
    public async Task<NomVO[]> GetNomsByTermAsync(
        [FromRoute][SuppressMessage("", "IDE0060")]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromQuery] string? term,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IEntityNomsRepository<TopicPlanPublisher> repository,
        CancellationToken ct)
        => await repository.GetNomsByTermAsync(term, offset, limit, ct);
}
