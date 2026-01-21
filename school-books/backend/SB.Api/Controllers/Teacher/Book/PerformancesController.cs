namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IPerformancesQueryRepository;

public class PerformancesController : BookAdminController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IPerformancesQueryRepository performancesQueryRepository,
        CancellationToken ct)
        => await performancesQueryRepository.GetAllAsync(schoolYear, classBookId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreatePerformanceAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] CreatePerformanceCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
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

    [HttpGet("{performanceId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int performanceId,
        [FromServices] IPerformancesQueryRepository performancesQueryRepository,
        CancellationToken ct)
        => await performancesQueryRepository.GetAsync(schoolYear, classBookId, performanceId, ct);

    [HttpPost("{performanceId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int performanceId,
        [FromBody] UpdatePerformanceCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PerformanceId = performanceId,
            },
            ct);

    [HttpDelete("{performanceId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int performanceId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemovePerformanceCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PerformanceId = performanceId,
            },
            ct);
}
