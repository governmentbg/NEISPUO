namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IGradeResultsQueryRepository;

public class GradeResultsController : BookAdminController
{
    [HttpGet]
    public async Task<ActionResult<GetAllVO[]>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromServices]IGradeResultsQueryRepository gradeResultsQueryRepository,
        CancellationToken ct)
        => await gradeResultsQueryRepository.GetAllAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<ActionResult<GetAllEditVO[]>> GetAllEditAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromServices]IGradeResultsQueryRepository gradeResultsQueryRepository,
        CancellationToken ct)
        => await gradeResultsQueryRepository.GetAllEditAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<ActionResult<GetSessionAllVO[]>> GetSessionAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromServices]IGradeResultsQueryRepository gradeResultsQueryRepository,
        CancellationToken ct)
        => await gradeResultsQueryRepository.GetSessionAllAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<ActionResult<GetSessionAllEditVO[]>> GetSessionAllEditAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromServices]IGradeResultsQueryRepository gradeResultsQueryRepository,
        CancellationToken ct)
        => await gradeResultsQueryRepository.GetSessionAllEditAsync(schoolYear, classBookId, ct);

    [HttpPost]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]UpdateGradeResultsCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("sessions")]
    public async Task UpdateSessionsAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]UpdateGradeResultSessionsCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);
}
