namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IStateExamDutyProtocolsQueryRepository;

public class StateExamDutyProtocolsController : ProtocolsController
{
    [HttpGet("{stateExamDutyProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int stateExamDutyProtocolId,
        [FromServices] IStateExamDutyProtocolsQueryRepository stateExamDutyProtocolsQueryRepository,
        CancellationToken ct)
        => await stateExamDutyProtocolsQueryRepository.GetAsync(schoolYear, stateExamDutyProtocolId, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateStateExamDutyProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{stateExamDutyProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int stateExamDutyProtocolId,
        [FromBody] UpdateStateExamDutyProtocolCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                StateExamDutyProtocolId = stateExamDutyProtocolId,
            },
            ct);

    [HttpDelete("{stateExamDutyProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int stateExamDutyProtocolId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveStateExamDutyProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                StateExamDutyProtocolId = stateExamDutyProtocolId,
            },
            ct);
}
