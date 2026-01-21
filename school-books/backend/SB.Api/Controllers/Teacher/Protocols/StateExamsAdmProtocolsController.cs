namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IStateExamsAdmProtocolQueryRepository;

public class StateExamsAdmProtocolsController : ProtocolsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] AdmProtocolType? protocolType,
        [FromQuery] string? protocolNum,
        [FromQuery] DateTime? protocolDate,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices]IStateExamsAdmProtocolQueryRepository stateExamsAdmProtocolQueryRepository,
        CancellationToken ct)
        => await stateExamsAdmProtocolQueryRepository.GetAllAsync(instId, schoolYear, protocolType, protocolNum, protocolDate, offset, limit, ct);

    [HttpGet("{stateExamsAdmProtocolId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int stateExamsAdmProtocolId,
        [FromServices] IStateExamsAdmProtocolQueryRepository stateExamsAdmProtocolQueryRepository,
        CancellationToken ct)
        => await stateExamsAdmProtocolQueryRepository.GetAsync(schoolYear, stateExamsAdmProtocolId, ct);

    [HttpGet("{stateExamsAdmProtocolId:int}")]
    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int stateExamsAdmProtocolId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices]IStateExamsAdmProtocolQueryRepository stateExamsAdmProtocolQueryRepository,
        CancellationToken ct)
        => await stateExamsAdmProtocolQueryRepository.GetStudentAllAsync(schoolYear, stateExamsAdmProtocolId, offset, limit, ct);

    [HttpGet("{stateExamsAdmProtocolId:int}/{classId:int}/{personId:int}")]
    public async Task<ActionResult<GetStudentVO>> GetStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int stateExamsAdmProtocolId,
        [FromRoute] int classId,
        [FromRoute] int personId,
        [FromServices]IStateExamsAdmProtocolQueryRepository stateExamsAdmProtocolQueryRepository,
        CancellationToken ct)
        => await stateExamsAdmProtocolQueryRepository.GetStudentAsync(schoolYear, stateExamsAdmProtocolId, classId, personId, ct);

    [HttpPost]
    public async Task<int> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody]CreateStateExamsAdmProtocolCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost("{stateExamsAdmProtocolId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int stateExamsAdmProtocolId,
        [FromBody]UpdateStateExamsAdmProtocolCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                StateExamsAdmProtocolId = stateExamsAdmProtocolId,
            },
            ct);

    [HttpPost("{stateExamsAdmProtocolId:int}")]
    public async Task CreateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int stateExamsAdmProtocolId,
        [FromBody]CreateStateExamsAdmProtocolStudentCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                StateExamsAdmProtocolId = stateExamsAdmProtocolId,
            },
            ct);

    [HttpPost("{stateExamsAdmProtocolId:int}")]
    public async Task UpdateStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int stateExamsAdmProtocolId,
        [FromBody]UpdateStateExamsAdmProtocolStudentCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                StateExamsAdmProtocolId = stateExamsAdmProtocolId,
            },
            ct);

    [HttpDelete("{stateExamsAdmProtocolId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int stateExamsAdmProtocolId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveStateExamsAdmProtocolCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                StateExamsAdmProtocolId = stateExamsAdmProtocolId,
            },
            ct);

    [HttpDelete("{stateExamsAdmProtocolId:int}")]
    public async Task RemoveStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int stateExamsAdmProtocolId,
        [FromQuery] int classId,
        [FromQuery] int personId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveStateExamsAdmProtocolStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                StateExamsAdmProtocolId = stateExamsAdmProtocolId,
                ClassId = classId,
                PersonId = personId,
            },
            ct);
}
