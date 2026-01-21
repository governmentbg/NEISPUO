namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISanctionsQueryRepository;

public class SanctionsController : BookAdminController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ISanctionsQueryRepository sanctionsQueryRepository,
        CancellationToken ct)
        => await sanctionsQueryRepository.GetAllAsync(schoolYear, classBookId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateSanctionAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]CreateSanctionCommand command,
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

    [HttpGet("{sanctionId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int sanctionId,
        [FromServices]ISanctionsQueryRepository sanctionsQueryRepository,
        CancellationToken ct)
        => await sanctionsQueryRepository.GetAsync(schoolYear, classBookId, sanctionId, ct);

    [HttpPost("{sanctionId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int sanctionId,
        [FromBody]UpdateSanctionCommand command,
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
                SanctionId = sanctionId,
            },
            ct);

    [HttpDelete("{sanctionId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int sanctionId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSanctionCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SanctionId = sanctionId,
            },
            ct);
}
