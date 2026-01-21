namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IPublicationsQueryRepository;

[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class InfoBoardController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllPublishedVO>>> GetAllPublishedAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]bool archived,
        [FromQuery]PublicationType? type,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IPublicationsQueryRepository publicationsQueryRepository,
        CancellationToken ct)
        =>  await publicationsQueryRepository.GetAllPublishedAsync(schoolYear, instId, archived, type, offset, limit, ct);

    [HttpGet]
    public async Task<ActionResult<GetMetadataVO>> GetMetadataAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]PublicationType? type,
        [FromServices]IPublicationsQueryRepository publicationsQueryRepository,
        CancellationToken ct)
        =>  await publicationsQueryRepository.GetMetadataAsync(schoolYear, instId, type, ct);

    [HttpGet("{publicationId:int}")]
    public async Task<ActionResult<GetPublishedVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int publicationId,
        [FromServices]IPublicationsQueryRepository publicationsQueryRepository,
        CancellationToken ct)
        => await publicationsQueryRepository.GetPublishedAsync(schoolYear, instId, publicationId, ct);
}
