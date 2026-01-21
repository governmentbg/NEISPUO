namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IStudentPublicationsQueryRepository;

[Authorize(Policy = Policies.StudentAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class StudentInfoBoardController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetStudentPublicationsVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]bool archived,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices] IStudentPublicationsQueryRepository publicationsQueryRepository,
        CancellationToken ct)
        =>  await publicationsQueryRepository.GetStudentPublicationsAsync(schoolYear, instId, archived, offset, limit, ct);

    [HttpGet]
    public async Task<ActionResult<GetStudentPublicationsMetadataVO>> GetMetadataAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromServices] IStudentPublicationsQueryRepository publicationsQueryRepository,
        CancellationToken ct)
        =>  await publicationsQueryRepository.GetStudentPublicationsMetadataAsync(schoolYear, instId, ct);

    [HttpGet("{publicationId:int}")]
    public async Task<ActionResult<GetStudentPublicationVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int publicationId,
        [FromServices] IStudentPublicationsQueryRepository publicationsQueryRepository,
        CancellationToken ct)
        => await publicationsQueryRepository.GetStudentPublicationAsync(schoolYear, instId, publicationId, ct);
}
