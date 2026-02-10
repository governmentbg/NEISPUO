namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IFirstGradeResultsQueryRepository;

public class FirstGradeResultsController : BookAdminController
{
    [HttpGet]
    public async Task<GetAllVO[]> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] IFirstGradeResultsQueryRepository firstGradeResultsQueryRepository,
        CancellationToken ct)
        => await firstGradeResultsQueryRepository.GetAllAsync(schoolYear, classBookId, ct);

    [HttpPost]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] UpdateFirstGradeResultCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId()
            },
            ct);
}
